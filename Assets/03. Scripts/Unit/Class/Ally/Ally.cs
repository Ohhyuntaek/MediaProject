using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Tilemaps;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    [Header("Raycaster 설정 (캐릭터 자식에 배치)")]
    [SerializeField] private RaycastTileHighlighter2D leftRaycaster;   // 캐릭터 왼쪽(발 쪽)에 배치
    [SerializeField] private RaycastTileHighlighter2D rightRaycaster;  // 캐릭터 오른쪽(머리 쪽)에 배치
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float tileWidth = 1.0f;
    [SerializeField] private float tileHeight =  1.0f;

    private int _lastKnockbackEnemyCount = 0;
    private StateMachine<Ally> _stateMachine;
    private float _lifeTimer;
    private ISkill<Ally> _skill;
    [SerializeField] private bool _isDead=false;
    private int _totalDamage = 0;
    private bool _finalSkill = false; // 스킬을 단 한번만 사용할 수 있는 캐릭터시 사용
    private bool _revived = false; // 부활한 적이 있으면 true 아직 안햇으면 false;
    [SerializeField]private bool _isOnTile = false;
    private float _totallifeTime = 0;
    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;
    
    public AllyType allyType;
    private Vector3 occupiedTilePosition;

    public void Init(Vector3 tilePosition)
    {
        occupiedTilePosition = tilePosition;
        _isOnTile = true;
        _isDead = false;
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
        GetFlip();
       
    }

    public void Initialize(UnitData data)
    {
        _unitData = data;
        _lifeTimer = _unitData.Duration;
        _skill = CreateSkillFromData(_unitData.AllySkillType);

        _stateMachine = new StateMachine<Ally>(this);
        _stateMachine.ChangeState(new AllyIdleState(1/_unitData.AttackSpeed));
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

                enemy[0].TakeDamage(_unitData.BaseAttack);

            }
            else
            {
                foreach (Enemy target in enemy)
                {
                    target.TakeDamage(_unitData.BaseAttack);
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
            
            // 타일 반환
            TileManager.Instance.FreeTile(occupiedTilePosition);
            
            // 오브젝트 풀에 복귀
            AllyPoolManager.Instance.ReturnAlly(allyType, this.gameObject);
        }
    }

    [CanBeNull]
    private ISkill<Ally> CreateSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.MovementBlock => new MovementBlockSkill(),
            AllySkillType.Debuff => new DebuffSkill(),
            AllySkillType.DamageDealer => new DamageSkill(),
            AllySkillType.KnockBack => new KnockbackSkill(),
            AllySkillType.NightLord => new NightLordSkill(),
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

    public List<Enemy> DetectTargets(int range)
    {
        List<Enemy> targets = new List<Enemy>();

        // 자식에 있는 RaycastTileHighlighter2D 컴포넌트를 가져옵니다.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        RaycastTileHighlighter2D tileHighlighter = null;
        if (spriteRenderer != null && spriteRenderer.flipX)
            tileHighlighter = rightRaycaster;
        else
            tileHighlighter = leftRaycaster;
        if (tileHighlighter == null)
        {
            Debug.LogWarning("RaycastTileHighlighter2D 컴포넌트를 찾을 수 없습니다.");
            return targets;
        }

        tileHighlighter.DetectTiles(range);

        // hitCellPos(레이로 맞춘 타일)이 있다면 그 셀부터 검사합니다.
        if (tileHighlighter.hitCellPos.HasValue)
        {
            // 1. hitTile (레이로 맞춘 타일) 자체를 검사
            Vector3Int hitTile = tileHighlighter.hitCellPos.Value;
            if (tileHighlighter._tilemap.HasTile(hitTile))
            {
                Vector3 center = tileHighlighter._tilemap.GetCellCenterWorld(hitTile);
                Collider2D[] cols = Physics2D.OverlapPointAll(center, _enemyLayer);
                foreach (Collider2D col in cols)
                {
                    Debug.Log($"{gameObject.name}이 {col.name} (hitTile) 찾았다");
                    Enemy enemy = col.GetComponent<Enemy>();
                    if (enemy != null && !targets.Contains(enemy))
                        targets.Add(enemy);
                }
            }

            // 2. hitTile의 오른쪽으로 확장된 타일들 검사 (rangeCells)
            if (tileHighlighter.rangeCells != null)
            {
                foreach (Vector3Int cell in tileHighlighter.rangeCells)
                {
                    if (!tileHighlighter._tilemap.HasTile(cell))
                        continue;

                    Vector3 cellCenter = tileHighlighter._tilemap.GetCellCenterWorld(cell);
                    Collider2D[] cols = Physics2D.OverlapPointAll(cellCenter, _enemyLayer);
                    foreach (Collider2D col in cols)
                    {
                        Debug.Log($"{gameObject.name}이 {col.name} (rangeCell) 찾았다");
                        Enemy enemy = col.GetComponent<Enemy>();
                        if (enemy != null && !targets.Contains(enemy))
                            targets.Add(enemy);
                    }
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

    public void ApllyDamageMulti(List<Enemy> target,int atk)
    {
        _totalDamage = 0;
        foreach (Enemy enemy in target)
        {
            enemy.TakeDamage(atk);
            _totalDamage += atk;
        }
    }

    public void ApplyKnockback(List<Enemy> targets)
    {
        // TODO: 대상들에게 넉백효과 구현 필요
    }

    public void ApplyBuffByEnemyCount(int enemyCount, BuffType buffType)
    {
        //TODO : 넉백된 수 만큼 혹은 공격한 수만큼 버프 효과 적용
        Debug.Log($"{buffType} 효과 적용" );
        // 1. 스폰된 리스트에 버프효과 적용
        // 2. 버프 타입마다 다르게 적용 필요 혹은 캐릭터마다
        
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

   
}
