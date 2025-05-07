
public class BountyHunterSpecialAttackState : IState<Ally>
{
    public void Enter(Ally ally)
    {
       ally.Animator.SetTrigger("5_SpecialAttack");
       
    }

    public void Update(Ally ally)
    {
       
    }

    public void TranstionTo(Ally ally)
    {
        
    }

    public void Exit(Ally ally)
    {
        
    }
}
