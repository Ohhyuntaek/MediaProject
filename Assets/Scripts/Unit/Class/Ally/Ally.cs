using System.Collections;
using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    private StateMachine<Ally> _stateMachine;
    private float _lifeTimer;
    private ISkill _skill;
    
    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;
    
    
    /// <summary>
    /// 씬에 직접 배치시에만 스폰할때 Initialize 호출해줘야함.
    /// 테스트용
    /// </summary>
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
        if (_lifeTimer <= 0f)
        {
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
            Debug.LogWarning($"[Ally:{name}] 스킬이 null입니다! 스킬 타입: {_unitData.AllySkillType}");
            return;
        }
        _skill.Activate(this);
    }

    public void PerformDie()
    {
        StartCoroutine(Die());
    }
    
    private ISkill CreateSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.MovementBlock => new MovementBlockSkill(),
            AllySkillType.Debuff => new DebuffSkill(),
            AllySkillType.DamageDealer => new DamageSkill(),
            _ => null
        };
    }
    
    public IEnumerator Die()
    {
        Debug.Log($"[Ally:{name}] 사망");
        yield return null;
        
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        yield return new WaitForSeconds(animLength);
        Destroy(gameObject,0.6f);
    }
}