using System.Collections;
using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private Animator _animator;
    private AllyStateMachine _stateMachine;
    private float _lifeTimer;
    
    public Animator Animator => _animator;
    public UnitData UnitData => _unitData;
    
    
    private void Start()
    {
        // 씬에서 직접 배치했을 경우만
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
        

        _stateMachine = new AllyStateMachine();
        _stateMachine.ChangeState(new AllyIdleState(this));
    }

    private void Update()
    {
        _stateMachine?.Update();

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            _stateMachine.ChangeState(new AllyDeadState(this));
        }
    }

    public void ChangeState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void PerformAttack()
    {
        Debug.Log($"[Ally:{name}] {_unitData.UnitName} 공격 시도!");

        switch (_unitData.AllySkillType)
        {
            case AllySkillType.MovementBlock:
                Debug.Log("적 이동 제약 효과!");
                break;
            case AllySkillType.DamageDealer:
                Debug.Log("피해 입힘!");
                break;
            case AllySkillType.Debuff:
                Debug.Log("디버프 효과!");
                break;
            case AllySkillType.AllyBuff:
                Debug.Log("아군 강화!");
                break;
            default:
                Debug.Log("기본 공격");
                break;
        }
    }

    public void PerformDie()
    {
        StartCoroutine(Die());
    }
    public IEnumerator Die()
    {
        Debug.Log($"[Ally:{name}] 사망");
        yield return null;
        
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log($"[Ally:{name}] 죽는 애니메이션 길이: {animLength}");

        yield return new WaitForSeconds(animLength);
        Destroy(gameObject,0.6f);
    }
}