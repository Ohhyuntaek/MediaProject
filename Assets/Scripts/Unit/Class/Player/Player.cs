using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;

    private StateMachine<Player> _stateMachine;
    private bool _canUseSkill = true;
    private float _skillCooldownTimer;
    private ISkill _skill;

    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;

    public bool CanUseSkill => _canUseSkill;
    public float SkillCoolTime => _unitData.SkillCoolTime;

    public void Initialize(UnitData data)
    {
        _unitData = data;
        _skill = CreateSkillFromData(_unitData.PlayerSkillType);
        _stateMachine = new StateMachine<Player>(this);
        _stateMachine.ChangeState(new PlayerIdleState());
    }

    private void Update()
    {
        _stateMachine?.Update();

        
        if (!_canUseSkill)
        {
            _skillCooldownTimer -= Time.deltaTime;
            if (_skillCooldownTimer <= 0f)
            {
                _canUseSkill = true;
            }
        }
    }

    public void ChangeState(IState<Player> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void PerformSkill()
    {
        Debug.Log($"[Player:{name}] 스킬 발동!");
        

        _canUseSkill = false;
        _skillCooldownTimer = _unitData.SkillCoolTime;
    }

    public void PerformDie()
    {
        StartCoroutine(Die());
    }
    
    private ISkill CreateSkillFromData(PlayerSkillType skillType)
    {
        return skillType switch
        {
            PlayerSkillType.Test1 => new MovementBlockSkill(),
            PlayerSkillType.Test2 => new DebuffSkill(),
            PlayerSkillType.Test3 => new DamageSkill(),
            _ => null
        };
    }
    
    public IEnumerator Die()
    {
        Debug.Log($"[Player:{name}] 사망");
        yield return null;
        
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        yield return new WaitForSeconds(animLength);
        Destroy(gameObject,0.6f);
    }
}