using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    
    private int _lastKnockbackEnemyCount = 0;
    private StateMachine<Ally> _stateMachine;
    private float _lifeTimer;
    private ISkill<Ally> _skill;
    private bool _isDead=false;

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
        _stateMachine?.Update();

        _lifeTimer -= Time.deltaTime;
        if (!_isDead && _lifeTimer <= 0f)
        {
            _isDead = true;
            _stateMachine.ChangeState(new AllyDeadState());
        }
    }

    public void ChangeState(IState<Ally> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void PerformAttack()
    {
        if (_skill == null)
        {
            Debug.LogWarning($"[Ally:{name}] 스킬이 null");
            return;
        }

        _skill.Activate(this);
    }

    public void PerformDie()
    {
        StartCoroutine(Die());
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
            _ => null
        };
    }

    public IEnumerator Die()
    {
        Debug.Log($"[Ally:{name}] 사망");
        yield return null;

        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        Destroy(gameObject);
    }

    

    public Enemy DetectSingleEnemy()
    {
        // TODO: 가장 가까운 적 1명 탐지
        return null;
    }

    public List<Enemy> DetectTargets(int range)
    {
        // TODO: 범위 내 적 탐지 후 최대 N명까지 반환
        return new List<Enemy>();
    }

    public void ApplyDamage(Enemy target)
    {
        // TODO: 데미지 계산 및 피격 처리
        Debug.Log($"[Ally] {target.name}에게 {UnitData.BaseAttack} 데미지!");
    }

    public void ApplyKnockback(List<Enemy> targets)
    {
        // TODO: 대상들에게 넉백효과 구현 필요
    }

    public void ApplyAllyAttackSpeedBuff(int enemyCount)
    {
        if (UnitData.UnitName != "KnockbackWarrior") return;
        
        //_animator.SetTrigger("3_Buff");
        float bonus = enemyCount * 5f;
        Debug.Log($"[잔다르크 특능] 아군 공격속도 +{bonus}%");
    }
    public void SetLastKnockbackEnemyCount(int count) => _lastKnockbackEnemyCount = count;
    public int GetLastKnockbackEnemyCount() => _lastKnockbackEnemyCount;

    
}
