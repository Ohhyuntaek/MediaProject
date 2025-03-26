using System;
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
    private float _hp;

    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;

    public bool CanUseSkill => _canUseSkill;
    public float SkillCoolTime => _unitData.SkillCoolTime;


    /// <summary>
    /// 씬에 직접 배치시에만 스폰할때 Initialize 호출해줘야함.
    /// 상관없으려나 암튼 테스트 필요
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
        _skill = CreateSkillFromData(_unitData.PlayerSkillType);
        _hp = _unitData.MaxHP;
        _stateMachine = new StateMachine<Player>(this);
        _stateMachine.ChangeState(new PlayerIdleState());
    }

    private void Update()
    {
        _stateMachine?.Update();
        if (_hp < 0 || Input.GetKey(KeyCode.Space))
        {
            _stateMachine.ChangeState(new PlayerDeadState());
        }
        
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
        
        _skill.Activate(this);
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
        Destroy(gameObject,animLength);
    }
}