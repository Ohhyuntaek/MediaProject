using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitData _unitData;
    private ISkill _skill;
    private StateMachine<Enemy> _stateMachine;
    [SerializeField] private Transform _target; // 일단 테스트용으로 타겟을 확정적으로 줘보장
    private float _attackCooldown;
    private float _moveSpeed;
    private float _attackRange;
    private float _hp;

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
            Debug.LogWarning($"[Enemy:{name}] UnitData가 할당되지 않았습니다.");
        }
    }

    public void Initialize(UnitData data)
    {
    
        _unitData = data;
        _attackCooldown = 1f / _unitData.AttackSpeed;
        _moveSpeed = _unitData.MoveSpeed;
        _attackRange = _unitData.AttackRange;
        _hp = _unitData.MaxHP;
        _skill = CreateSkillFromData(_unitData.EnemySkillType);
        _stateMachine = new StateMachine<Enemy>(this);
        _stateMachine.ChangeState(new EnemyWalkState());
    }

    private void Update()
    {
        _stateMachine?.Update();

        
        if (_hp < 0f)
        {
            _stateMachine.ChangeState(new EnemyDeadState());
        }
    }

    public void ChangeState(IState<Enemy> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void MoveForward()
    {
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
    }

    public bool IsTargetInRange()
    {
        if (_target == null) return false;
        return Vector3.Distance(transform.position, _target.position) <= _attackRange;
    }

    public void PerformAttack()
    {
        Debug.Log($"[Enemy:{name}] 공격!");
        
    }

    public void PerformDie()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        Debug.Log($"[Enemy:{name}] 사망");
        yield return null;

        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        Destroy(gameObject, 0.6f);
    }
    private ISkill CreateSkillFromData(EnemySkillType skillType)
    {
        return skillType switch
        {
            EnemySkillType.None => new None(),
            EnemySkillType.AllyDisruptor => new AllyDisruptor(),
            EnemySkillType.AllySupporter => new AllySupporter(),
            EnemySkillType.PlayerDebuffer => new PlayerDebuffer(),
            _ => null
        };
    }

    public void ApplyStun(float duration)
    {
        ChangeState(new EnemyStunState(duration));
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}