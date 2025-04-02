using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Android;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    
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

        _stateMachine = new StateMachine<Ally>(this);
        _stateMachine.ChangeState(new AllyIdleState());
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
            }
            Debug.Log($"[Ally:{name}] 사망 시작");
            Debug.Log($"[Ally:{name}] 오브젝트 제거!");
            _isDead = true;
        }
        
        if (_isDead)
            Destroy(gameObject);
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
        // TODO: 범위 내 적 탐지 후 최대 3명까지 반환
        return new List<Enemy>();
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

    public List<Enemy> DetectNearestEnemyTileEnemies()
    {
        //TODO: 가장 가까운 적 리스트 찾기
        return new List<Enemy>();
    }
}
