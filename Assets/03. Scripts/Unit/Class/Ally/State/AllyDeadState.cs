using UnityEngine;

public class AllyDeadState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.Animator?.SetTrigger("4_Death");
        
    }

    public void Update(Ally ally)
    {
        ally.PerformDie();
    }

    public void Exit(Ally ally) { }
}