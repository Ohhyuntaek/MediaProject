using UnityEngine;

public class AllyIdleState : IState
{
    
    private Ally _ally;
    private float _attackTimer;

    public AllyIdleState(Ally ally)
    {
        _ally = ally;
    }
    
    public void Enter()
    {
        _attackTimer = 1f / _ally.UnitData.AttackSpeed;
        _ally.Animator?.SetBool("1_Move",false);
    }

    void IState.Update()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f ) // ++ 적감지시)
        {
            _ally.ChangeState(new AllyAttackState(_ally));
        }
    }

    public void Exit()
    {
        
    }

    void Update()
    {
        
    }
}
