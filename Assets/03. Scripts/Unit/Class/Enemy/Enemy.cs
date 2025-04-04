using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _destination; 
    private ISkill<Enemy> _skill;
    private StateMachine<Enemy> _stateMachine;

    private float _attackCooldown;
    private float _moveSpeed;
    private float _attackRange;
    private float _hp;

    public Animator Animator => _animator;
    public EnemyData EnemyData => _enemyData;
    public Transform Target => _target; // 타겟 접근용

    private void Start()
    {
        if (_enemyData != null)
        {
            Initialize(_enemyData);
        }
        else
        {
            Debug.LogWarning($"[{name}] EnemyData가 할당되지 않았습니다.");
        }
    }

    public void Initialize(EnemyData data)
    {
        _enemyData = data;
        _hp = _enemyData.MaxHp;
        _moveSpeed = _enemyData.MoveSpeed;
        _attackCooldown = 1f / _enemyData.AtkSpeed;
        _attackRange = _enemyData.ATKRange;  
        _skill = CreateSkillFromData(_enemyData.Skill);
        _stateMachine = new StateMachine<Enemy>(this);
        _stateMachine.ChangeState(new EnemyWalkState(_destination));
        
    }

    private void Update()
    {
        _stateMachine?.Update();

        if (_hp <= 0)
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
        if (_target == null)
            return;

        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    
    public bool IsTargetInRange()
    {
        if (_target == null)
            return false;
        return Vector3.Distance(transform.position, _target.position) <= _attackRange;
    }

    // 공격 수행 메서드 (상태 전환에 따라 호출)
    public void PerformAttack()
    {
        Debug.Log($"[{name}] 공격 수행!");
        // 예시: 타겟에게 데미지 적용 등 추가 로직 구현
        _skill.Activate(this);
    }

    // 외부에서 데미지를 받을 때 호출
    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _stateMachine.ChangeState(new EnemyDeadState());
        }
    }

 
    public void PerformDie()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Death") && stateInfo.normalizedTime > 0.9f)
        {
            Destroy(gameObject,0.5f);
        }
    }

   

    private ISkill<Enemy> CreateSkillFromData(EnemySkill skillType)
    {
        return skillType switch
        {
            //EnemySkill.None => new NoneSkill<Enemy>(),
            //EnemySkill.Basic3 => new Basic3Skill(),
            _ => null
        };
    }

    // 스턴 효과 적용 (예시)
    public void ApplyStun(float duration)
    {
        ChangeState(new EnemyStunState(duration));
    }

    public void ApplyDebuff(DebuffType type)
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        if (_destination != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _destination.position);
        }
    }

    
    public Animator EnemyAnimatior => _animator;
}
