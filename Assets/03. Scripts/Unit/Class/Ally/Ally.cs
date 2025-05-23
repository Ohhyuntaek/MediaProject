using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Android;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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
    [SerializeField]private List<PolygonCollider2D> _patternColliders;
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
    private bool _isSpawnEnd = false;
    
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
        _isOnTile = false;
        _isDead = false;
        _revived = false;
        _isSpawnEnd = false;
        _maxDuration = UnitData.Duration;
        _DurationSlider.maxValue = _maxDuration;
        _DurationSlider.value = _maxDuration;
        GetFlip();
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
        _stateMachine.ChangeState(new AllySpawnState());
        
    }
   
    private void Update()
    {
        _stateMachine?.Update();
        if (_isSpawnEnd)
        {
            _duration -= Time.deltaTime;
            _totallifeTime += Time.deltaTime;
            if (_unitData.UnitName == "NightLord" && !_revived && _duration<=0f && !_isDead)
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
        
    }
    public void InitPatternColliders()
    {
        // 1) 초기화
        _patternColliders = new List<PolygonCollider2D>();
        if (_occupiedTile == null || _occupiedTile._hitCollider == null)
        {
              return;
        } 
        

        // 2) 소환된 타일(히트셀) 콜라이더 가져오기
        PolygonCollider2D hitCellCollider = _occupiedTile._hitCollider;

        // 3) GridTargetManager 에서 해당 콜라이더의 행·열 인덱스 얻기
        if (!GridTargetManager.Instance.TryGetGridIndex(hitCellCollider, out int baseRow, out int baseCol))
            return;
        Debug.Log("히트셀 위치 인덱스 " + baseRow +" "+baseCol);

        // 4) DetectionPattern 만큼 오프셋 순회
        var pattern = UnitData.DetectionPatternSo.cellOffsets;
        foreach (var ofs in pattern)
        {
            
            var applied = (_occupiedTile.dir)  // dir=false → up, dir=true → down (flip 여부에 따라)
                ? new Vector2Int(ofs.x, -ofs.y)   
                : new Vector2Int(ofs.x, ofs.y); 

            int row = baseRow + applied.y;
            int col = baseCol + applied.x;

            // 4-2) 범위 검사
            if (row < 0 || row >= GridTargetManager.Instance.coliderMat.Length) 
                continue;
            if (col < 0 || col >= GridTargetManager.Instance.coliderMat[row].arr_row.Length) 
                continue;

            // 4-3) 해당 슬롯의 PolygonCollider2D 추가
            var poly = GridTargetManager.Instance.coliderMat[row].arr_row[col];
            if (poly != null)
                _patternColliders.Add(poly);
        }
    }

    public void ChangeState(IState<Ally> newState)
    {
        if (_isDead) return; 
        _stateMachine.ChangeState(newState);
    }

    public void PerformAttack()
    {
        
        List<IDamageable> targets = DetectTargets();
        if (targets.Count == 0) 
            return;

        
        if (_unitData.TargetingType == TargetingType.Single)
        {
            var damageableTarget = targets[0] as MonoBehaviour;
            ParticleManager.Instance.PlayAttackParticle(_unitData.AllyType,damageableTarget.transform.position );
            targets[0].TakeDamage(_baseAttack);
            
            
        }
        else
        {
           
            foreach (IDamageable t in targets)
            {
                var target = t as MonoBehaviour;
                ParticleManager.Instance.PlayAttackParticle(_unitData.AllyType,target.transform.position);
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
            Debug.Log($"[Ally:{name}] 사망 애니메이션 완료 → 풀로 반환");

            _isDead = true;
            
            if (_occupiedTile != null)
            {
                _occupiedTile.isOccupied = false;
                _occupiedTile.ally = null;
                _occupiedTile = null;
            }
            
            // 오브젝트 풀에 복귀
            InGameSceneManager.Instance.allyPoolManager.ReturnAlly(_allyType, gameObject);
        }
    }

    private void OnEnable()
    {
        InitPatternColliders();
        
    }

    public void ForceDie()
    {
        _revived = true; // 부활 막기
        _duration = 0f;
        _isDead = true;
        _stateMachine.ChangeState(new AllyDeadState());
    }

    [CanBeNull]
    private ISkill<Ally> CreateSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.CentaurLadySkill => new CentaurLadySkill(),
            AllySkillType.BountyHunterSkill => new BountyHunterSkill(),
            AllySkillType.SalamenderSkill => new SalamenderSkill(),
            AllySkillType.JoanDarcSkill => new JandarkSkill(),
            AllySkillType.NightLordSkill => new NightLordSkill(),
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
    
    public List<IDamageable> DetectTargets()
    {
        var targets = new List<IDamageable>();
        if (_patternColliders == null) 
            return targets;

        // 중복 방지용 임시 버퍼
        var buffer = new Collider2D[30];
        var filter = new ContactFilter2D();
        filter.SetLayerMask(_enemyLayer);
        filter.useTriggers = true;

        // 1) 미리 계산해둔 패턴 콜라이더 리스트 순회
        foreach (var poly in _patternColliders)
        {
            // 해당 폴리곤 안에 있는 모든 콜라이더를 가져옴
            int count = poly.Overlap(filter, buffer);
            for (int i = 0; i < count; i++)
            {
                var col = buffer[i];
                if (col == null) continue;

                // 2) IDamageable 인터페이스를 구현한 몬스터(Enemy, Boss 등)만 추가
                if (col.TryGetComponent<IDamageable>(out var dmg) 
                    && !targets.Contains(dmg))
                {
                    targets.Add(dmg);
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

    public void ApllyDamageMulti(List<IDamageable> target)
    {
        _totalDamage = 0;
        foreach (Enemy enemy in target)
        {
            enemy.TakeDamage(_baseAttack);
            _totalDamage += _baseAttack;
        }
    }

   
  public void GrabEnemies(List<IDamageable> targets)
{
    // 1) 내 소환 타일(히트셀) 인덱스 구하기
    if (_occupiedTile == null || _occupiedTile._hitCollider == null) 
        return;
    if (!GridTargetManager.Instance.TryGetGridIndex(
            _occupiedTile._hitCollider,
            out int baseRow,
            out int baseCol))
        return;

    // 2) 미리 “같은 행”과 “다른 행”용 destPoly 두 개 계산
    var mat = GridTargetManager.Instance.coliderMat;
    int destColIndex = baseCol + 1;  // 언제나 X+1 열
    if (destColIndex < 0 || destColIndex >= mat[0].arr_row.Length)
        return;

    // 같은 행용
    PolygonCollider2D sameRowPoly = null;
    if (baseRow >= 0 && baseRow < mat.Length)
        sameRowPoly = mat[baseRow].arr_row[destColIndex];

    // 다른 행용 (위/아래 반대)
    // _occupiedTile.dir == false 면 “up”행 → otherRow = baseRow+1
    // _occupiedTile.dir == true  면 “down”행 → otherRow = baseRow-1
    int otherRow = _occupiedTile.dir ? baseRow - 1 : baseRow + 1;
    PolygonCollider2D otherRowPoly = null;
    if (otherRow >= 0 && otherRow < mat.Length)
        otherRowPoly = mat[otherRow].arr_row[destColIndex];

    // 3) 반복문 안에서는 대상이 속한 행(enemyRow)에 따라
    //    미리 구한 sameRowPoly or otherRowPoly 로 이동
    foreach (var dmg in targets)
    {
        // 보스면 CC만
        if (dmg is Boss b)
        {
            b.TakeDamage(_baseAttack);
            b.ApplyCC();
            continue;
        }

        if (!(dmg is Enemy enemy)) 
            continue;

        // 데미지, 스턴
        enemy.TakeDamage(_baseAttack);
        enemy.ApplyStun(1.5f);

        // IDamageable → MonoBehaviour 캐스트
        var mb = (MonoBehaviour)dmg;

        // 4) 적이 속한 타일의 행 인덱스 (enemyRow) 얻기
        if (!GridTargetManager.Instance.TryGetColliderAtPosition(
                (Vector2)mb.transform.position,
                out var srcPoly))
            continue;
        if (!GridTargetManager.Instance.TryGetGridIndex(
                srcPoly,
                out int enemyRow,
                out _))
            continue;

        // 5) 적이 같은 행이라면 sameRowPoly, 아니면 otherRowPoly
        var destPoly = (enemyRow == baseRow) 
            ? sameRowPoly 
            : otherRowPoly;
        if (destPoly == null) 
            continue;

        // 6) 순간이동
        mb.transform.position = destPoly.bounds.center;
    }
}

   

    public void ApplyKnockback(List<IDamageable> targets)
    {
        float knockbackDistance = 1f;
        float knockbackDuration = 0.5f;

        foreach (var dmg in targets)
        {
            
            var mb = dmg as MonoBehaviour;
            if (mb == null) continue;
            float threshold = Random.Range(0, 0.3f);
            Transform destination = null;
            if (dmg is Enemy e)      destination = e.GetDestination();
            else if (dmg is Boss b)
            {
                ParticleManager.Instance.PlaySkillParticle(_allyType,b.transform.position+new Vector3(0,threshold,0),0);
                b.ApplyCC();
            }
            //SoundManager.Instance.PlaySfx(_unitData.AttackSound,transform.position,false);
            
            // 넉백 벡터 계산
            

            if (mb is Enemy)
            {
                Vector3 knockDir    = (mb.transform.position - destination.position).normalized;
                Vector3 displacement = knockDir * knockbackDistance;
                ParticleManager.Instance.PlaySkillParticle(_allyType,mb.transform.position+new Vector3(0,threshold,0),0);
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
        List<GameObject> _spawnList = InGameSceneManager.Instance.allyPoolManager.activateAllies;
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

        if (_occupiedTile.dir)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        SetOntile();

    }

    public void SetOntile()
    {
        if (_occupiedTile.lineType.ToString() == _unitData.UnitType.ToString())
        {
            _isOnTile = true;
        }
        else
        {
            _isOnTile = false;
        }
    }

    public void ToDestroy()
    {
        _duration = 0;
    }

    public void ForTest()
    {
        StartCoroutine(JustWaitForTest());
    }
    public IEnumerator JustWaitForTest()
    {
        yield return new WaitForSeconds(0.1f);
        _isSpawnEnd = true;
        Debug.Log("1초 대기후 스폰 상태 끝 아이들로 전환");
        ChangeState(new  AllyIdleState(1/_atkSpd));
    }

    public bool Dead => _isDead;
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

    public bool SpawnEnd
    {
        get => _isSpawnEnd;
        set => _isSpawnEnd = value;
    }
    public void SetSkillRandomNum(int value) => skillNumByRandom = value;
    public int GetSkillRandomNum() => skillNumByRandom;
    public int BASEATTACK => _baseAttack;
    public void SetBaseAttack(int change) => _baseAttack = change;
    public bool Dircetion => _dir;


}
