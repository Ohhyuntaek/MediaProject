using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Animator _animator;
    
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
    public Animator Animator => _animator;
    public PlayerData PlayerData => _playerData;
    public bool CanUseActiveSkill => _canUseActiveSkill;
    public float ActiveSkillCooldownTime => _playerData.ActiveSkillCooldown; 

    private void Start()
    {
        if(_playerData != null)
        {
            Initialize(_playerData);
        }
        else
        {
            Debug.LogWarning($"[{name}] UnitData가 할당되지 않았습니다.");
        }
    }

    public void Initialize(PlayerData data)
    {
        _playerData = data;
        _hp = _playerData.MaxHP;
        _activeSkill = CreateActiveSkillFromData(_playerData.ActiveSkillType);
        _passiveSkill = CreatePassiveSkillFromData(_playerData.ActiveSkillType);
        _chargeSpd = _playerData.ChargingSpd;
        _MaxEnergy = _playerData.MaxEnergy;
        _currentEnergy = _playerData.InitialEnergy;
        _stateMachine = new StateMachine<Player>(this);
        _stateMachine.ChangeState(new PlayerIdleState());
    }

    private void Update()
    {
        _stateMachine?.Update();

        if(_hp <= 0 &&!_isDead)
        {
            _isDead = true;
            _stateMachine.ChangeState(new PlayerDeadState());
        }

        
        if(!_canUseActiveSkill)
        {
            _activeSkillCooldownTimer -= Time.deltaTime;
            if(_activeSkillCooldownTimer <= 0f)
                _canUseActiveSkill = true;
        }

        _energyTimer += Time.deltaTime; 

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
            AllySkillType.Jandark => new JanDarkPlayerActiveSkill()
        };
    }
    
    private ISkill<Player> CreatePassiveSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            //AllySkillType.Jandark => new JanDarkPlayerActiveSkill()
            _ => null
        };
    }
    public int Energy {get => _currentEnergy; set => _currentEnergy = value;}
}
