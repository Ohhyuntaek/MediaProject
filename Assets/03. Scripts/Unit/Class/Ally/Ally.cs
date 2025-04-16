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
    private float _lifeTimer;
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
    
    [SerializeField]
    private Slider _SpawnTimeSlider;

    private float _maxSpawnTime;

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
        _maxSpawnTime = UnitData.Duration;
        _SpawnTimeSlider.maxValue = _maxSpawnTime;
        _SpawnTimeSlider.value = _maxSpawnTime;
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
        _lifeTimer = _unitData.Duration;
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

        _lifeTimer -= Time.deltaTime;
        _totallifeTime += Time.deltaTime;
        if (_unitData.UnitName == "NightLord" && !_revived && _lifeTimer<=0f)
        {
            _revived = true;
            _lifeTimer = 8f;
            _stateMachine.ChangeState(new AllyReviveState());
            return;
        }
        
        if (_SpawnTimeSlider != null)
        {
            _SpawnTimeSlider.value = _lifeTimer;
        }
        
        if (!_isDead && _lifeTimer <= 0f)
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
        List<Enemy> enemy = DetectTargets(_unitData.AttackRange);
        if(enemy.Count==0) return;
        if (enemy.Count > 0)
        {
            if (_unitData.TargetingType == TargetingType.Single)
            {

                enemy[0].TakeDamage(_baseAttack);

            }
            else
            {
                foreach (Enemy target in enemy)
                {
                    target.TakeDamage(_baseAttack);
                }
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

   

    public List<Enemy> DetectTargets(int range)
    {
        List<Enemy> targets = new List<Enemy>();
        // 자식에 있는 RaycastTileHighlighter2D를 가져옵니다.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        RaycastTileHighlighter2D tileHighlighter = null;
        
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            tileHighlighter = rightRaycaster;
            _dir = true;
        }
        else
            tileHighlighter = leftRaycaster;
        if (tileHighlighter == null)
        {
            Debug.LogWarning("RaycastTileHighlighter2D 컴포넌트를 찾을 수 없습니다.");
            return targets;
        }

        // 인자로 받은 range 값을 사용해 DetectTiles()를 호출하여 hitCellPos와 rangeCells 등 내부 데이터를 업데이트
        tileHighlighter.DetectTiles(range);

        // hitCellPos(레이로 맞춘 타일)가 있다면,
        if (tileHighlighter.hitCellPos.HasValue)
        {
            // hitCellPos를 중심으로 맨해튼 다이아몬드 영역 내의 모든 셀을 구합니다.
            Vector3Int centerTile = tileHighlighter.hitCellPos.Value;
            List<Vector3Int> cellsToCheck = new List<Vector3Int>();

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                    {
                        Vector3Int cell = new Vector3Int(centerTile.x + dx, centerTile.y + dy, centerTile.z);
                        // 해당 셀에 타일이 존재하는 경우에만 검사 대상에 추가
                        if (tileHighlighter._tilemap.HasTile(cell))
                            cellsToCheck.Add(cell);
                    }
                }
            }

            // 각 셀의 영역 전체(OverlapBoxAll)를 검사합니다.
            // 아이소메트릭 타일맵에서는 타일이 회전되어 보이므로, 회전값 45° (필요시 보정)로 OverlapBoxAll를 호출합니다.
            
            foreach (Vector3Int cell in cellsToCheck)
            {
                Vector3 cellCenter = tileHighlighter._tilemap.GetCellCenterWorld(cell);
                // cellSize는 타일 하나의 크기입니다. 여유를 위해 약간 축소할 수 있습니다.
                Vector3 cellSize = tileHighlighter._tilemap.cellSize * 0.9f;

                Collider2D[] cols = Physics2D.OverlapBoxAll(cellCenter, cellSize, 0f, _enemyLayer);
                foreach (Collider2D col in cols)
                {
                    //Debug.Log($"{gameObject.name}이 셀 {cell}에서 {col.name} 을(를) 찾았다");
                    Enemy enemy = col.GetComponent<Enemy>();
                    if (enemy != null && !targets.Contains(enemy) && _dir == enemy.Direction)
                        targets.Add(enemy);
                }
            }
        }
            return targets;
    }




    public void ApllyDamageSingle(Enemy target)
    {
        // TODO: 데미지 계산
        _totalDamage += 30;

    }

    public void ApllyDamageMulti(List<Enemy> target)
    {
        _totalDamage = 0;
        foreach (Enemy enemy in target)
        {
            enemy.TakeDamage(_baseAttack);
            _totalDamage += _baseAttack;
        }
    }

    public void ApplyKnockback(List<Enemy> targets)
    {
        float knockbackDistance = 1f;
        float knockbackDuration = 0.5f;
    
        foreach (Enemy enemy in targets)
        {
            Transform destination = enemy.GetDestination();
            if (destination == null)
            {
                Debug.LogWarning($"{enemy.name}의 도착지 Transform이 없습니다.");
                continue;
            }
            
            Vector3 knockDirection = (enemy.transform.position - destination.position).normalized;
            Vector3 displacement = knockDirection * knockbackDistance;
        
            enemy.StartCoroutine(SmoothKnockback(enemy, displacement, knockbackDuration));
        }
    }

    private IEnumerator SmoothKnockback(Enemy enemy, Vector3 displacement, float duration)
    {
        Vector3 startPos = enemy.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            enemy.transform.position = Vector3.Lerp(startPos, startPos + displacement, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        enemy.transform.position = startPos + displacement;
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
    public List<Enemy> DetectNearestEnemyTileEnemies()
    {
        List<Enemy> nearestEnemies = new List<Enemy>();

        // 씬에 존재하는 모든 Enemy 오브젝트들을 가져옵니다.
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        if (allEnemies.Length == 0)
            return nearestEnemies;

        // Ally 자신의 위치를 기준으로 가장 가까운 적을 찾습니다.
        Vector3 allyPos = transform.position;
        Enemy nearestEnemy = null;
        float minDistSqr = Mathf.Infinity;
        foreach (Enemy e in allEnemies)
        {
            float distSqr = (e.transform.position - allyPos).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                nearestEnemy = e;
            }
        }

        // 가장 가까운 적의 타일을 찾기 위해, rayHighcaster가 사용하는 타일맵을 참조합니다.
        // (왼쪽 혹은 오른쪽 rayHighcaster 중 하나의 타일맵을 사용합니다.)
        Tilemap tm = null;
        if (leftRaycaster != null && leftRaycaster._tilemap != null)
            tm = leftRaycaster._tilemap;
        else if (rightRaycaster != null && rightRaycaster._tilemap != null)
            tm = rightRaycaster._tilemap;
    
        if (tm == null)
        {
            Debug.LogWarning("적군 타일맵을 찾을 수 없습니다.");
            return nearestEnemies;
        }

        // 가장 가까운 적의 위치를 타일 좌표로 변환합니다.
        Vector3Int targetCell = tm.WorldToCell(nearestEnemy.transform.position);

        // 모든 적들 중에서, 해당 타일(Cell)에 위치한 적들을 반환합니다.
        foreach (Enemy e in allEnemies)
        {
            Vector3Int cell = tm.WorldToCell(e.transform.position);
            if (cell == targetCell && !nearestEnemies.Contains(e))
            {
                nearestEnemies.Add(e);
            }
        }

        return nearestEnemies;
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
    public void SetLifetime(float time) => _lifeTimer += time;
    public int GetTotalDamage => _totalDamage;
    public void SetTotalDamage(int damage) => _totalDamage = damage;
    public bool GetRevive() => _revived;
    public float GetTotalLifeTime => _totallifeTime;
    public void SetFinalSkill(bool use)
    {
        _finalSkill = use;
    }

    public void SetSkillRandomNum(int value) => skillNumByRandom = value;
    public int GetSkillRandomNum() => skillNumByRandom;
    public int BASEATTACK => _baseAttack;
    public void SetBaseAttack(int change) => _baseAttack = change;
    public bool Dircetion => _dir;


}
