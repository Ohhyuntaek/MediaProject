using System;
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

    private float _atkSpd;

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
    private int skillNumByRandom = 0;
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
        _atkSpd = _unitData.AttackSpeed;
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

    // 인자로 받은 range 값을 사용해 레이캐스트를 수행하여 hitCellPos와 rangeCells를 업데이트 합니다.
    // (기존 코드에서는 rangeCells에 오른쪽으로만 확장되었지만, 여기서는 중심 타일부터 좌우로 범위를 확장합니다.)
    tileHighlighter.DetectTiles(range);

    // hitCellPos(레이로 맞춘 타일)가 존재하면,
    if (tileHighlighter.hitCellPos.HasValue)
    {
        // centerTile : 레이로 맞춘 타일
        Vector3Int centerTile = tileHighlighter.hitCellPos.Value;
        List<Vector3Int> cellsToCheck = new List<Vector3Int>();

        // centerTile에서 좌측(-range)부터 우측(+range)까지 총 2 * range + 1 칸을 검사합니다.
        for (int dx = -range; dx <= range; dx++)
        {
            Vector3Int cell = new Vector3Int(centerTile.x + dx, centerTile.y, centerTile.z);
            // 해당 셀에 타일이 존재하는지 확인 (타일이 없다면 건너뜁니다)
            if (tileHighlighter._tilemap.HasTile(cell))
            {
                cellsToCheck.Add(cell);
            }
        }

        // 각 셀의 중심에서 enemyLayer에 포함된 모든 콜라이더를 검사하여 Enemy 컴포넌트를 가져옵니다.
        foreach (Vector3Int cell in cellsToCheck)
        {
            Vector3 cellCenter = tileHighlighter._tilemap.GetCellCenterWorld(cell);
            Collider2D[] cols = Physics2D.OverlapPointAll(cellCenter, _enemyLayer);
            foreach (Collider2D col in cols)
            {
                Debug.Log($"{gameObject.name}이 셀 {cell}에서 {col.name} 을(를) 찾았다");
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null && !targets.Contains(enemy))
                {
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
        float knockbackDistance = 0.5f;
        float knockbackDuration = 0.2f;
    
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
            Debug.Log("스피드 버프 적용 " + _ally.name);
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


}
