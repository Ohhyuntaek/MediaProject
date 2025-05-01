using System;
using System.Collections;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _destination;
    [SerializeField] private Transform leftSpawnPosition;
    [SerializeField] private Transform rightSpawnPosition;  
    private ISkill<Enemy> _skill;
    private StateMachine<Enemy> _stateMachine;
    private bool _isDead = false;
    private bool _dir=false;
    private float _atkSpeed;
    private float _moveSpeed;
    private float _attackRange;
    private float _hp;
    private float _defense;
    private bool _hide = false;
    private float _damage;
    private Dawn _dawn;
    [SerializeField] private Slider hpSlider;
    
    public Animator Animator => _animator;
    public EnemyData EnemyData => _enemyData;
    public Transform Target => _target; // 타겟 접근용

    private void Awake()
    {
        leftSpawnPosition = GameObject.Find("LeftEnemySpawn").transform;
        rightSpawnPosition = GameObject.Find("RightEnemySpawn").transform;
        

    }

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
        _atkSpeed = _enemyData.AtkSpeed;
        _attackRange = _enemyData.ATKRange;
        _damage = _enemyData.Damage;
        _defense = _enemyData.Deffense;
        _dawn = GameObject.FindFirstObjectByType<Dawn>();
        _skill = CreateSkillFromData(_enemyData.Skill);
        if (_destination.gameObject.name.Contains("Right")) _dir = true;
        _stateMachine = new StateMachine<Enemy>(this);
        _stateMachine.ChangeState(new EnemyWalkState());
        _target = _dawn.transform;
        hpSlider.maxValue = _hp;
    }

    private void Update()
    {
        _stateMachine?.Update();

        hpSlider.value = _hp;
        
        if (_hp <= 0 && !_isDead)
        {
            _isDead = true;
            _stateMachine.ChangeState(new EnemyDeadState());
        }
    }

    public void ChangeState(IState<Enemy> newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    public bool IsTargetInRange()
    {
        if (Vector3.Distance(transform.position, _destination.position) < 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 공격 수행 메서드 (상태 전환에 따라 호출)
    public void PerformAttack()
    {   
        
        if (IsTargetInRange())
        {   
             //SoundManager.Instance.PlaySfx(_enemyData.AttackSound,transform.position,false);
            _dawn.TakeDamage(_damage);
        }
    }

    public void PerformSkill()
    {
        _skill.Activate(this);
    }

    // 외부에서 데미지를 받을 때 호출
    public void TakeDamage(int damage)
    {
        if (!_hide)
        {
            _hp -= damage;
            //Debug.Log("데미지 받음 ㅠㅠ ");
            if (_hp <= 0)
            {
                _stateMachine.ChangeState(new EnemyDeadState());
            }
        }
    }
 
    public void PerformDie()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 0.9f)
        {
            gameObject.SetActive(false);
            InGameSceneManager.Instance.darkSpawner.DecreaseDarkCount();
        }
    }

    public void PerformHide(float time)
    {
        StartCoroutine(Hide(time));
    }
    
    public IEnumerator Hide(float time)
    {
        _hide = true;
        yield return new WaitForSeconds(time);
        _hide = false;
    }
   

    private ISkill<Enemy> CreateSkillFromData(EnemySkill skillType)
    {
        return skillType switch
        {
            //EnemySkill.None => new NoneSkill<Enemy>(),
            EnemySkill.Basic3 => new Basic3(),
            _ => null
        };
    }

    // 스턴 효과 적용 (예시)
    public void ApplyStun(float duration)
    {
        ChangeState(new EnemyStunState(duration));
    }

    public void SpawAttackEffect(Vector3 spawnPosition,int skullnum)
    {
        GameObject effectPrefeb = _enemyData.EnemyEffect[skullnum];
        GameObject effectObject =Instantiate(effectPrefeb, spawnPosition, effectPrefeb.transform.rotation);
        effectObject.GetComponent<Basic3Projectie>().SetCaster(this);
        
    }
    
    public void ApplyDebuff(DebuffType type,float duration)
    {
        switch (type)
        {
            case DebuffType.Slow:
                _moveSpeed--;
                break;
            case DebuffType.DamageAmp:
                _defense--;
                break;
            case DebuffType.Stun:
                ApplyStun(duration);
                break;
        }
    }
  
    /// <summary>
    /// 적군에게 버프 혹은 디버프를 적용하는 함수
    /// </summary>
    /// <param name="times"></param>
    /// <param name="duration"></param>
    /// <param name="buffOrDebuff"> 버프인 경우라면 true, 디버프라면 false 값을 준다.</param>
    public void ApplySpeedBuffDebuff(float times , float duration,bool buffOrDebuff)
    {
        
        float originalSpeed = _enemyData.MoveSpeed;
        if (buffOrDebuff)
        {
            _moveSpeed = originalSpeed * times;
        }
        else
        {
            _moveSpeed = originalSpeed / times;
            Debug.Log("moveSpeed 감소 " + originalSpeed + " "+ _moveSpeed);
        }
        
        StartCoroutine(RemoveSpeedBuffDebuffAfter(duration, originalSpeed));
    }
    private IEnumerator RemoveSpeedBuffDebuffAfter(float duration, float originalSpeed)
    {
        yield return new WaitForSeconds(duration);
        _moveSpeed = originalSpeed;
    }

    public void ApplyDefenseBuffDebuff(float times, float duration, bool buffOrDebuff)
    {
        float originalDefense = -_enemyData.Deffense;
        if (buffOrDebuff)
        {
            _defense = originalDefense * times;
        }
        else
        {
            originalDefense = originalDefense / times;
        }

        StartCoroutine(RemoveDeffenseBuffDebuffAfter(duration, originalDefense));
    }
    
    private IEnumerator RemoveDeffenseBuffDebuffAfter(float duration, float originalDefense)
    {
        yield return new WaitForSeconds(duration);
        _defense = originalDefense;
    }

    public void DealToPlayer()
    {
        _dawn.TakeDamage(_damage);
    }
    

    public void SetDestinationWhenSpawn(bool dir)
    {
        if (!dir)
        {
            _destination = GameObject.Find("LeftDestination").transform;
        }
        else
        {
            _destination = GameObject.Find("RightDestination").transform;
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        if (_destination != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _destination.position);
        }
    }

    public Transform GetDestination() => _destination;
    public float AttakcDamage => _damage;
    public float AtkSpeed => _atkSpeed;
    public Animator EnemyAnimatior => _animator;
    public float MoveSpeed => _moveSpeed;
    public bool Direction => _dir;
}
