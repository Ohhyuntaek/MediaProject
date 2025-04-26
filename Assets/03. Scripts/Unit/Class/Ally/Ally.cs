using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Android;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    [Header("Raycaster 설정 (캐릭터 자식에 배치)")]
    [SerializeField] private RaycastTileHighlighter2D leftRaycaster;   // 캐릭터 왼쪽(발 쪽)에 배치
    [SerializeField] private RaycastTileHighlighter2D rightRaycaster;  // 캐릭터 오른쪽(머리 쪽)에 배치
    [SerializeField] private LayerMask _enemyLayer;
     private float tileWidth = 0.9f;
    private float tileHeight =  0.9f;

    private float _atkSpd;

    private int _lastKnockbackEnemyCount = 0;
    private StateMachine<Ally> _stateMachine;
    private float _duration;
    private bool _CanCC = true;
    private ISkill<Ally> _skill;
    [SerializeField] private bool _isDead=false;
    private int _baseAttack;
    private int _totalDamage = 0;
    private bool _finalSkill = false; // 스킬을 단 한번만 사용할 수 있는 캐릭터시 사용
    private bool _revived = false; // 부활한 적이 있으면 true 아직 안햇으면 false;
    [SerializeField]private bool _isOnTile = false;
    private float _totallifeTime = 0;
    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;
    
    private Vector3 occupiedTilePosition;
    private int skillNumByRandom = 0;
    
    [FormerlySerializedAs("_SpawnTimeSlider")] [SerializeField]
    private Slider _DurationSlider;

    private float _maxDuration;

    private bool _dir =false;
    
    [FormerlySerializedAs("allyType")] public AllyType _allyType;
    
    [SerializeField]
    private AllyTile _occupiedTile; // Ally 오브젝트가 소환된 타일 이름

    public void Init(Vector3 position, AllyTile tile)
    {
        transform.position = position;
        _occupiedTile = tile;
        tile.ally = this;
        _isOnTile = true;
        _isDead = false;
        _maxDuration = UnitData.Duration;
        _DurationSlider.maxValue = _maxDuration;
        _DurationSlider.value = _maxDuration;
        
        // 추가: Duration 초기화
        if (_unitData != null)
        {
            _duration = _unitData.Duration;
            _maxDuration = _unitData.Duration;
        }
        else
        {
            Debug.LogWarning($"[{name}] UnitData가 설정되어 있지 않아서 Duration을 초기화할 수 없습니다.");
        }

        // Duration UI 슬라이더 초기화
        if (_DurationSlider != null)
        {
            _DurationSlider.maxValue = _maxDuration;
            _DurationSlider.value = _maxDuration;
        }
    }

    public void ApplyTileBonus()
    {
        Debug.Log("ApplyTileBonus");
    }
   
    
    private void Start()
    {
        
        if (_unitData != null)
        {
            Initialize(_unitData);
        }
        else
        {
            Debug.LogWarning($"[{name}] UnitData가 할당되지 않았습니다.");
        }
       
       
    }

    public void Initialize(UnitData data)
    {
        _unitData = data;
        _duration = _unitData.Duration;
        _skill = CreateSkillFromData(_unitData.AllySkillType);
        _atkSpd = _unitData.AttackSpeed;
        _baseAttack = _unitData.BaseAttack;
        _stateMachine = new StateMachine<Ally>(this);
        _stateMachine.ChangeState(new AllyIdleState(1/_atkSpd));
        GetFlip();
    }

    private void Update()
    {
        _stateMachine?.Update();

        _duration -= Time.deltaTime;
        _totallifeTime += Time.deltaTime;
        if (_unitData.UnitName == "NightLord" && !_revived && _duration<=0f)
        {
            _revived = true;
            _duration = 8f;
            _stateMachine.ChangeState(new AllyReviveState());
            return;
        }
        
        if (_DurationSlider != null)
        {
            _DurationSlider.value = _duration;
        }
        
        if (!_isDead && _duration <= 0f)
        {
            _stateMachine.ChangeState(new AllyDeadState());
        }
    }

    public void ChangeState(IState<Ally> newState)
    {
        if (_isDead) return; 
        _stateMachine.ChangeState(newState);
    }

    public void PerformAttack()
    {
        
        List<IDamageable> targets = DetectTargets(_unitData.AttackRange);
        if (targets.Count == 0) 
            return;

       
        if (_unitData.TargetingType == TargetingType.Single)
        {
            targets[0].TakeDamage(_baseAttack);
        }
        else
        {
           
            foreach (IDamageable t in targets)
            {
                t.TakeDamage(_baseAttack);
            }
        }
    }


    public void PerformSkill()
    {
        if (_isDead)
        {
            Debug.Log($"[Ally:{name}] 이미 사망 상태이므로 공격 불가");
            return;
        }

        if (_skill == null)
        {
            Debug.LogWarning($"[Ally:{name}] 스킬이 null");
            return;
        }

        _skill.Activate(this);
    }
    
    

    public void PerformDie()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 0.9f)
        {
            if (_isDead)
            {
                Debug.Log("[Ally] 이미 사망 상태입니다.");
                return;
            }
            Debug.Log($"[Ally:{name}] 사망 시작 및 반환 처리");
            _isDead = true;

            if (_occupiedTile != null)
            {
                _occupiedTile.isOccupied = false;
                _occupiedTile.ally = null;
                _occupiedTile = null;
            }
            
            // 오브젝트 풀에 복귀
            AllyPoolManager.Instance.ReturnAlly(_allyType, this.gameObject);
        }
    }

    [CanBeNull]
    private ISkill<Ally> CreateSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.CentaurLady => new CentaurLadySkill(),
            AllySkillType.BountyHunter => new BountyHunterSkill(),
            AllySkillType.Salamender => new SalamenderSkill(),
            AllySkillType.Jandark => new JandarkSkill(),
            AllySkillType.NightLord => new NightLordSkill(),
            AllySkillType.None => new NoneSkill(),
            _ => null
        };
    }

    public bool OnTile => _isOnTile;

    public void SetOnTile(bool _tile)
    {
        _isOnTile = true;
    }

    public Enemy DetectSingleEnemy()
    {
        // TODO: 가장 가까운 적 1명 탐지
        return null;
    }

    public List<Ally> GetAroundAllies()
    {
        return _occupiedTile.GetAroundAlly();
    }
    
    public List<IDamageable> DetectTargets(int range)
    {
        var targets = new List<IDamageable>();

        // 1) Raycaster 선택
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        RaycastTileHighlighter2D tileHighlighter = (spriteRenderer != null && spriteRenderer.flipX)
            ? rightRaycaster
            : leftRaycaster;
        _dir = spriteRenderer.flipX;

        if (tileHighlighter == null)
        {
            Debug.LogWarning("RaycastTileHighlighter2D 컴포넌트를 찾을 수 없습니다.");
            return targets;
        }

        // 2) 타일 감지
        tileHighlighter.DetectTiles(range);
        if (!tileHighlighter.hitCellPos.HasValue)
            return targets;

        var center = tileHighlighter.hitCellPos.Value;
        var cellsToCheck = new List<Vector3Int>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                {
                    var cell = new Vector3Int(center.x + dx, center.y + dy, center.z);
                    if (tileHighlighter._tilemap.HasTile(cell))
                        cellsToCheck.Add(cell);
                }
            }
        }

        // 3) 각 셀 영역에서 Enemy 또는 Boss 감지
        foreach (var cell in cellsToCheck)
        {
            Vector3 cellCenter = tileHighlighter._tilemap.GetCellCenterWorld(cell);
            Vector3 cellSize   = tileHighlighter._tilemap.cellSize * 1f;
            var cols = Physics2D.OverlapBoxAll(cellCenter, cellSize, 0f, _enemyLayer);

            foreach (var col in cols)
            {
                // Enemy 검사
                if (col.TryGetComponent<Enemy>(out var e) && !targets.Contains(e))
                {
                    if (_dir == col.gameObject.GetComponent<Enemy>().Direction)
                    {
                        targets.Add(e);
                    }
                    
                }
                    

                // Boss 검사
                if (col.TryGetComponent<Boss>(out var b) && !targets.Contains(b))
                    targets.Add(b);
            }
        }
        
        return targets;
    }


   

   




    public void ApllyDamageSingle(Enemy target)
    {
        // TODO: 데미지 계산
        _totalDamage += 30;

    }

    public void ApllyDamageMulti(List<IDamageable> target)
    {
        _totalDamage = 0;
        foreach (Enemy enemy in target)
        {
            enemy.TakeDamage(_baseAttack);
            _totalDamage += _baseAttack;
        }
    }

    public void ApplyKnockback(List<IDamageable> targets)
    {
        float knockbackDistance = 1f;
        float knockbackDuration = 0.5f;

        foreach (var dmg in targets)
        {
            // IDamageable이 실제로는 Enemy 또는 Boss이므로,
            // MonoBehaviour로 캐스팅해서 transform과 StartCoroutine을 얻습니다.
            var mb = dmg as MonoBehaviour;
            if (mb == null) continue;

            // Enemy/Boss 양쪽에서 제공하는 GetDestination() 호출
            Transform destination = null;
            if (dmg is Enemy e)      destination = e.GetDestination();
            else if (dmg is Boss b)
            {
                b.ApplyCC();
            }
            
            
            // 넉백 벡터 계산
            

            if (mb is Enemy)
            {
                Vector3 knockDir    = (mb.transform.position - destination.position).normalized;
                Vector3 displacement = knockDir * knockbackDistance;
                mb.StartCoroutine(SmoothKnockback(mb.transform, displacement, knockbackDuration));
            }
           
        }
    }

    private IEnumerator SmoothKnockback(Transform target, Vector3 displacement, float duration)
    {
        Vector3 start = target.position;
        float   t     = 0f;

        while (t < duration)
        {
            target.position = Vector3.Lerp(start, start + displacement, t / duration);
            t               += Time.deltaTime;
            yield return null;
        }
        target.position = start + displacement;
    }
    public void ApplySpeedBuffDebuff(float times , float duration,bool buffOrDebuff)
    {
        
        float originalSpeed = _unitData.AttackSpeed;
        if (buffOrDebuff)
        {
             _atkSpd= originalSpeed * times;
        }
        else
        {
            _atkSpd = originalSpeed / times;
        }
        
        StartCoroutine(RemoveSpeedBuffDebuffAfter(duration, originalSpeed));
    }
    private IEnumerator RemoveSpeedBuffDebuffAfter(float duration, float originalSpeed)
    {
        yield return new WaitForSeconds(duration);
        _atkSpd = originalSpeed;
    }


    public void ApplyBuffByEnemyCount(int enemyCount, BuffType buffType)
    {
        //TODO : 넉백된 수 만큼 혹은 공격한 수만큼 버프 효과 적용
        List<GameObject> _spawnList = AllyPoolManager.Instance.activateAllies;
        float _increaseSpd = enemyCount * 0.05f + 1;
        foreach (GameObject _ally in _spawnList)
        {
            _ally.GetComponent<Ally>().ApplySpeedBuffDebuff(_increaseSpd,2f,true);
           
        }

    }
    public List<IDamageable> DetectNearestTileTargets()
    {
        // 1) 모은 모든 Enemy와 Boss를 찾아서 가장 가까운 한 기를 찾습니다.
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Boss[]   allBosses  = FindObjectsOfType<Boss>();

        Vector3   myPos       = transform.position;
        float     minDistSqr  = float.MaxValue;
        MonoBehaviour nearestMB      = null;
        IDamageable        nearestDmg = null;

        // Enemy 중에서
        foreach (var e in allEnemies)
        {
            float d = (e.transform.position - myPos).sqrMagnitude;
            if (d < minDistSqr)
            {
                minDistSqr  = d;
                nearestMB   = e;
                nearestDmg  = e;
            }
        }

        // Boss 중에서
        foreach (var b in allBosses)
        {
            float d = (b.transform.position - myPos).sqrMagnitude;
            if (d < minDistSqr)
            {
                minDistSqr  = d;
                nearestMB   = b;
                nearestDmg  = b;
            }
        }

       
        RaycastTileHighlighter2D tileHighlighter = null;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.flipX) tileHighlighter = rightRaycaster;
        else                         tileHighlighter = leftRaycaster;

        if (tileHighlighter == null || nearestMB == null)
            return new List<IDamageable>();

        Tilemap tm = tileHighlighter._tilemap;
        if (tm == null)
            return new List<IDamageable>();

      
        Vector3Int targetCell = tm.WorldToCell(nearestMB.transform.position);

      
        List<IDamageable> results = new List<IDamageable>();
        foreach (var e in allEnemies)
        {
            if (tm.WorldToCell(e.transform.position) == targetCell)
                results.Add(e);
        }
        foreach (var b in allBosses)
        {
            if (tm.WorldToCell(b.transform.position) == targetCell)
                results.Add(b);
        }

        return results;
    }


    public IEnumerator CanCCByDuration(float duration)
    {
        _CanCC = false;
        yield return new WaitForSeconds(duration);
        _CanCC = true;
    }

    public void SetCanCCByDuration(float duration)
    {
        StartCoroutine(CanCCByDuration(duration));
    }
    
    public void GetFlip()
    {

        if (leftRaycaster.GetTargetTileTag() == null)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

    }

    
    public float ATKSPD => _atkSpd;
    public void SetLastKnockbackEnemyCount(int count) => _lastKnockbackEnemyCount = count;
    public int GetLastKnockbackEnemyCount() => _lastKnockbackEnemyCount;
    public bool FinalSkill => _finalSkill;
    public void SetLifetime(float time) => _duration += time;
    public int GetTotalDamage => _totalDamage;
    public void SetTotalDamage(int damage) => _totalDamage = damage;
    public bool GetRevive() => _revived;
    public float GetTotalLifeTime => _totallifeTime;
    public void SetFinalSkill(bool use)
    {
        _finalSkill = use;
    }

    public AllyTile OccupiedTile => _occupiedTile;

    public void SetSkillRandomNum(int value) => skillNumByRandom = value;
    public int GetSkillRandomNum() => skillNumByRandom;
    public int BASEATTACK => _baseAttack;
    public void SetBaseAttack(int change) => _baseAttack = change;
    public bool Dircetion => _dir;


}
