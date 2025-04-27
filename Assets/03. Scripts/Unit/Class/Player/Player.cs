using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [FormerlySerializedAs("_playerData")] [SerializeField] private DawnData dawnData;
    [SerializeField] private Animator _animator;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _energySlider;
    
    private StateMachine<Player> _stateMachine;
    private bool _canUseActiveSkill = true;
    private float _activeSkillCooldownTimer;
    private ISkill<Player> _activeSkill;
    private ISkill<Player> _passiveSkill;
    private float _hp;
    private bool _isDead = false;
    private float _energyTimer = 0f;
    private int _chargeSpd;
    private int _currentEnergy;
    private int _MaxEnergy;
    private bool _CanUpdatePassive = true;

    public Animator Animator => _animator;
    public DawnData DawnData => dawnData;
    public bool CanUseActiveSkill => _canUseActiveSkill;
    public float ActiveSkillCooldownTime => dawnData.ActiveSkillCooldown;
    

    private void Start()
    {
        if(dawnData != null)
        {
            Initialize(dawnData);
        }
        else
        {
            Debug.LogWarning($"[{name}] UnitData가 할당되지 않았습니다.");
        }
    }

    public void Initialize(DawnData data)
    {
        _hpSlider.maxValue = dawnData.MaxHP;
        _energySlider.maxValue = dawnData.MaxEnergy;
        
        dawnData = data;
        _hp = dawnData.MaxHP;
        _activeSkill = CreateActiveSkillFromData(dawnData.ActiveSkillType);
        _passiveSkill = CreatePassiveSkillFromData(dawnData.ActiveSkillType);
        _chargeSpd = dawnData.ChargingSpd;
        _MaxEnergy = dawnData.MaxEnergy;
        _currentEnergy = dawnData.InitialEnergy;
        _stateMachine = new StateMachine<Player>(this);
        _stateMachine.ChangeState(new PlayerIdleState());
    }

    private void Update()
    {
        _stateMachine?.Update();
        UpdatePassiveSkill();
        
        if(_hp <= 0 &&!_isDead)
        {
            _isDead = true;
            _stateMachine.ChangeState(new PlayerDeadState());
        }
        else
        {   
            UpdatePassiveSkill();
            if(!_canUseActiveSkill)
            {
                _activeSkillCooldownTimer -= Time.deltaTime;
                if(_activeSkillCooldownTimer <= 0f)
                    _canUseActiveSkill = true;
            }
            _energyTimer += Time.deltaTime; 
            ChargeMana();
        }
        
        if (_hpSlider != null)
        {
            _hpSlider.value = _hp;
        }

        if (_energySlider != null)
        {
            _energySlider.value = _energyTimer;
        }

    }

    public void ChangeState(IState<Player> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    
    public void PerformActiveSkill()
    {
        Debug.Log($"[Player:{name}] 액티브 스킬 발동!");
        _activeSkill.Activate(this);
        _canUseActiveSkill = false;
        _activeSkillCooldownTimer = ActiveSkillCooldownTime;
    }
    
   
    public void UpdatePassiveSkill()
    {
        if(_passiveSkill != null)
        {   
            _passiveSkill.Activate(this);
            
        }
    }

    public void ChargeMana()
    {
        if (_energyTimer > _chargeSpd)
        {
            if (_currentEnergy < _MaxEnergy)
            {
                _currentEnergy = Mathf.Min(_currentEnergy+2, _MaxEnergy);
                _energyTimer = 0f;

            }
        }
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if(_hp <= 0)
        {
            ChangeState(new PlayerDeadState());
        }
    }

    
    public void PerformDie()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 0.9f)
        {
            _isDead = true;
            gameObject.SetActive(false);
        }
    }

    private ISkill<Player> CreateActiveSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.Joandarc => new JanDarkPlayerActiveSkill()
        };
    }
    
    private ISkill<Player> CreatePassiveSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.Joandarc => new JandarkPassiveSkill()
           
        };
    }
    public int Energy {get => _currentEnergy; set => _currentEnergy = value;}
    
}
