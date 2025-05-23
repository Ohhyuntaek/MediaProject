using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Boss : MonoBehaviour, IDamageable
{
    [Header("공유 컴포넌트")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _player;            // 플레이어 Transform
    [SerializeField] private EnemyData _bossData;
    [SerializeField] private Transform _bossDestination;
    

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
    private bool _lastspecial = false;
    [SerializeField] private GameObject _stunEffect;
    [SerializeField] private Slider hpSlider;

    private bool _jump = false;
    private float _attacckTimer;
    private bool _canAttack=false;
    private void Start()
    {
        _initialPosition = transform.position;
        _stateMachine = new StateMachine<Boss>(this);
        Initialize();
        _stateMachine.ChangeState(new BossIdleState());
    }

    private void Initialize()
    {
        _hp = _bossData.MaxHp;
        _mopveSpd = _bossData.MoveSpeed;
        _damage = _bossData.Damage;
        _attacckTimer = attackInterval;
        _bossDestination = GameObject.Find("BossDestination").transform;
        _stunEffect = transform.GetChild(0).gameObject;
        _stunEffect.SetActive(false);
        _player = GameObject.FindFirstObjectByType<Dawn>().transform;
        _direction = (_bossDestination.position - transform.position).normalized;
        hpSlider.maxValue = _hp;

    }
    private void OnDrawGizmos()
    {
        if (_player == null) 
            return;

        // 씬 뷰에 보스와 플레이어를 잇는 선을 그립니다.
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _bossDestination.position);

       
    }

    private void PerformDie()
    {
        InGameSceneManager.Instance.darkSpawner.DecreaseDarkCount();
        ChangeState(new BossDeadState());
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _skipNextMove = true;
        }
        if (!_canAttack)
        {
            _attacckTimer -= Time.deltaTime;
        }
        
        hpSlider.value = _hp;
        
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
        ParticleManager.Instance.TriggerVignetteAndShake();
        _player.GetComponent<Dawn>().TakeDamage(dmg);
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        Debug.Log("남은 hp" + _hp);
        if (_hp <= 0 && !_isDead)
        {
            _isDead = true;
            PerformDie();
        }
    }

    /// <summary>앞줄 아군 하나 즉시 소환해제</summary>
    public void DespawnFrontAlly()
    {
        List<Ally> _frontList = InGameSceneManager.Instance.allyPoolManager.GetLineObject_Spawned(LineType.Front);
        if (_frontList.Count > 0)
        {
            _frontList[0].ToDestroy();
        }
    }

    /// <summary>뒤줄 아군 하나 즉시 소환해제</summary>
    public void DespawnBackAlly()
    {
        List<Ally> _reartList = InGameSceneManager.Instance.allyPoolManager.GetLineObject_Spawned(LineType.Rear);
        if (_reartList.Count > 0)
        {
            _reartList[0].ToDestroy();
        }
    }

    /// <summary>전열 아군이 2기 이상인지</summary>
    public bool HasAtLeastFrontAllies(int count)
    {   
        List<Ally> _frontList = InGameSceneManager.Instance.allyPoolManager.GetLineObject_Spawned(LineType.Front);
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
                list[i].ToDestroy();
            }
            
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ToDestroy();
            }
        }
    }

    /// <summary>필드 위 모든 아군 파괴</summary>
    public void DestroyAllAllies()
    {
        List<GameObject> list = InGameSceneManager.Instance.allyPoolManager.activateAllies;
        foreach (GameObject allyobject in list)
        {
            Ally ally = allyobject.GetComponent<Ally>();
            
            ally.ToDestroy();
        }
    }
    public Ally GetClosestAlly(Vector3 origin)
    {
        List<GameObject> list = InGameSceneManager.Instance.allyPoolManager.activateAllies;
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

    public bool CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, _bossDestination.position);
        Debug.Log("너와 나의 거리 " + distance);
        if (distance<=0.01f)
        {
            Debug.Log("너무 가까워");
            return true;
        }
        else
        {   
            Debug.Log("너무 멀어");
            return false;
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
    public bool SkipNextMove
    {
        get => _skipNextMove;
        set => _skipNextMove = value;
    }

    public bool LastSpecialAttack
    {
        get => _lastspecial;
        set => _lastspecial = value;
    }

    public GameObject StunEffect => _stunEffect;
    public Transform BossDestiNation => _bossDestination;
    public Vector3 InitialPosition => _initialPosition;
    public Vector3 Dircetion => _direction;
    public Animator Animator => _animator;
    public EnemyData BossData => _bossData;

    #endregion
}
