using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Dawn : MonoBehaviour
{
    [FormerlySerializedAs("_playerData")] [SerializeField] private DawnData dawnData;
    [SerializeField] private Animator _animator;
    
    private StateMachine<Dawn> _stateMachine;
    private bool _canUseActiveSkill = true;
    [SerializeField]
    private float _activeSkillCooldownTimer;
    private ISkill<Dawn> _activeSkill;
    private ISkill<Dawn> _passiveSkill;
    private float _hp;
    private bool _isDead = false;
    private float _energyTimer = 0f;
    private int _chargeSpd;
    private float _currentEnergy;
    private int _MaxEnergy;
    private bool _CanUpdatePassive = true;

    private float cooldownMultiplier => RuntimeDataManager.Instance.enhancement.cooldownMultiplier;
    private float energyChargeMultiplier => RuntimeDataManager.Instance.enhancement.energyChargeMultiplier;
    
    public Animator Animator => _animator;
    public DawnData DawnData => dawnData;
    public bool CanUseActiveSkill => _canUseActiveSkill;
    public float ActiveSkillCooldownTime => dawnData.ActiveSkillCooldown;
    public float CurrentHP => _hp;
    public float currentEnergyUpSpeed = 0.1f;
    
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
        dawnData = data;
        _hp = dawnData.MaxHP;
        _activeSkill = CreateActiveSkillFromData(dawnData.ActiveSkillType);
        _passiveSkill = CreatePassiveSkillFromData(dawnData.ActiveSkillType);
        _chargeSpd = dawnData.ChargingSpd;
        _MaxEnergy = dawnData.MaxEnergy;
        _currentEnergy = dawnData.InitialEnergy;
        _stateMachine = new StateMachine<Dawn>(this);
        _stateMachine.ChangeState(new PlayerIdleState());
        ResetActiveCooldown();
    }

    private void Update()
    {
        _stateMachine?.Update();
        UpdatePassiveSkill();
        
        if(_hp <= 0 &&!_isDead)
        {
            _isDead = true;
            _stateMachine.ChangeState(new PlayerDeadState());
            InGameSceneManager.Instance.isGameOver = true;
        }
        else
        {   
            UpdatePassiveSkill();
            // 쿨다운 감소: 스테이지가 진행 중일 때만
            if (!_canUseActiveSkill && InGameSceneManager.Instance.stageManager.CurrentStageState == StageState.Playing)
            {
                _activeSkillCooldownTimer += Time.deltaTime * cooldownMultiplier;

                if (_activeSkillCooldownTimer >= ActiveSkillCooldownTime)
                {
                    _activeSkillCooldownTimer = ActiveSkillCooldownTime;
                    _canUseActiveSkill = true;
                }
            }
            _energyTimer += Time.deltaTime; 
            ChargeMana();
        }
    }

    public void ChangeState(IState<Dawn> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void useEnerge()
    {
        _currentEnergy -= dawnData.energyUseValue;
    }
    
    
    public void ResetActiveCooldown()
    {
        _canUseActiveSkill = false;
        _activeSkillCooldownTimer = 0f;
    }

    public float ActiveCooldownRatio
    {
        get
        {
            // 쿨다운이 진행 중이면 0 ~ 1 사이 비율 반환
            return Mathf.Clamp01(_activeSkillCooldownTimer / ActiveSkillCooldownTime);
            
            // 쿨다운 완료 상태면 항상 1
            // return 1f;
        }
    }
    
    public void PerformActiveSkill()
    {
        if (!_canUseActiveSkill) return;

        Debug.Log($"[Player:{name}] 액티브 스킬 발동!");
        _activeSkill.Activate(this);

        _canUseActiveSkill = false;
        _activeSkillCooldownTimer = 0f;  // 쿨다운 진행을 0부터 시작
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
                // 에너지 회복 배수 적용
                _currentEnergy = Mathf.Min(_currentEnergy + (currentEnergyUpSpeed * energyChargeMultiplier), _MaxEnergy);
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

    private ISkill<Dawn> CreateActiveSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.JoanDarcSkill => new JanDarkPlayerActiveSkill()
        };
    }
    
    private ISkill<Dawn> CreatePassiveSkillFromData(AllySkillType skillType)
    {
        return skillType switch
        {
            AllySkillType.JoanDarcSkill => new JandarkPassiveSkill()
           
        };
    }
    
    public float currentEnergy {get => _currentEnergy; set => _currentEnergy = value;}
    
}
