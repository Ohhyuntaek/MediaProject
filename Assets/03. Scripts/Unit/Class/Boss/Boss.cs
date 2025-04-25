using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boss : MonoBehaviour, IDamageable
{
    [Header("공유 컴포넌트")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _player;            // 플레이어 Transform
    [SerializeField] private EnemyData _bossData;

    [Header("이동 설정")]
    public float moveInterval = 10f; // 10초마다 이동
    public float attackInterval = 5f;
    public float tileDistance = 1f;        // 한 칸 거리
    private int _moveCount = 0;
    private Vector3 _initialPosition;
    private bool _skipNextMove = false;    // 버프 실패 혹은 CC 시 다음 이동 스킵
    private float _hp;
    private float _mopveSpd;
    private float _damage;
    private bool _isDead;
    private Vector3 _direction;
    private StateMachine<Boss> _stateMachine;



    private bool _jump = false;
    private float _attacckTimer;
    private bool _canAttack=false;
    private void Start()
    {
        _initialPosition = transform.position;
        _stateMachine = new StateMachine<Boss>(this);
        _direction = (_player.position - transform.position).normalized;
        Initialize();
        _stateMachine.ChangeState(new BossIdleState());
    }

    private void Initialize()
    {
        _hp = _bossData.MaxHp;
        _mopveSpd = _bossData.MoveSpeed;
        _damage = _bossData.Damage;
        _attacckTimer = attackInterval;

    }

    private void PerformDie()
    {
        ChangeState(new BossDeadState());
    }
    
    private void Update()
    {
        if (!_canAttack)
        {
            _attacckTimer -= Time.deltaTime;
        }
        
        
        if (_attacckTimer < 0f && !_canAttack)
        {
            _canAttack = true;
        }
       
        
        _stateMachine.Update();

    }
    public void ChangeState(IState<Boss> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    #region —— 행동 메서드들 ——

    
    
    /// <summary>플레이어에게 대미지</summary>
    public void DealDamageToPlayer(int dmg)
    {
       
        _player.GetComponent<Player>().TakeDamage(dmg);
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0 && !_isDead)
        {
            _isDead = true;
            PerformDie();
        }
    }

    /// <summary>앞줄 아군 하나 즉시 소환해제</summary>
    public void DespawnFrontAlly()
    {
        List<Ally> _frontList = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Front);
        if (_frontList.Count > 0)
        {
            _frontList[0].ChangeState(new AllyDeadState());
        }
    }

    /// <summary>뒤줄 아군 하나 즉시 소환해제</summary>
    public void DespawnBackAlly()
    {
        List<Ally> _reartList = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Rear);
        if (_reartList.Count > 0)
        {
            _reartList[0].ChangeState(new AllyDeadState());
        }
    }

    /// <summary>전열 아군이 2기 이상인지</summary>
    public bool HasAtLeastFrontAllies(int count)
    {   
        List<Ally> _frontList = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Front);
        if (_frontList.Count >=2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>전열 아군 중 랜덤 n기 소환해제</summary>
    public void DespawnRandomFrontAllies(List<Ally> list,int n)
    {
        if (list.Count > n)
        {
            for (int i = 0; i < n; i++)
            {
                list[i].ChangeState(new AllyDeadState());
            }
            
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ChangeState(new AllyDeadState());
            }
        }
    }

    /// <summary>필드 위 모든 아군 파괴</summary>
    public void DestroyAllAllies()
    {
        List<GameObject> list = AllyPoolManager.Instance.activateAllies;
        foreach (GameObject allyobject in list)
        {
            Ally ally = allyobject.GetComponent<Ally>();
            ally.ChangeState(new AllyDeadState());
        }
    }
    public Ally GetClosestAlly(Vector3 origin)
    {
        List<GameObject> list = AllyPoolManager.Instance.activateAllies;
        if (list.Count == 0)
        {
            return null;
        }
        
        GameObject closest = null;
        float minDistSqr = float.MaxValue;
        
        
        foreach (var allyObj in list)
        {
            if (allyObj == null) continue;

            // 거리 제곱 계산 (sqrt 대신 제곱을 비교해서 성능 최적화)
            float distSqr = (allyObj.transform.position - origin).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                closest = allyObj;
            }
        }

        return closest.GetComponent<Ally>();


    }

    /// <summary>CC(스턴) 당했을 때 호출</summary>
    public void ApplyCC()
    {
        if (_jump)
        {
            _skipNextMove = true;
        }
       
    }

    #endregion

    #region ----GET,SET모음------

    public float BossHp
    {
        get => _hp;
        set => _hp = value;
    }

    public float BossMoveSpeed
    {
        get => _mopveSpd;
        set => _mopveSpd = value;
    }

    public float BosssDamage
    {
        get => _damage;
        set => _damage = value;
    }

    public float MoveInterval
    {
        get => moveInterval;
    }

    public int MoveCount
    {
        get => _moveCount;
        set => _moveCount = value;
    }

    public void UpMoveCount()
    {
        _moveCount++;
    }

    public bool CanAttack
    {
        get => _canAttack;
       
    }

    public void InitializeAttack()
    {
        _canAttack = false;
        _attacckTimer = attackInterval;
    }


    public bool Jumping
    {
        get => _jump;
        set => _jump = value;
    }
    

    public Vector3 Dircetion => _direction;
    public bool SkipNextMove => _skipNextMove;
    public Animator Animator => _animator;


    #endregion
}
