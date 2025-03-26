
public class AllyAttackState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.Animator?.SetTrigger("2_Attack");
        ally.PerformAttack();
        ally.ChangeState(new AllyIdleState());
    }

    public void Update(Ally ally) { }
    public void Exit(Ally ally) { }
}