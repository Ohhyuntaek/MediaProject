using UnityEngine;

public class AllyAttackState : IState
{
    private Ally _ally;
    
    public AllyAttackState(Ally ally)
    {
        _ally = ally;
    }

    public void Enter()
    {
        _ally.Animator?.SetTrigger("2_Attack");
        _ally.PerformAttack();
        _ally.ChangeState(new AllyIdleState(_ally));
    }

    public void Update() { }
    public void Exit() { }
}
