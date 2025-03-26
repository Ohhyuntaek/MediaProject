using UnityEngine;

public class AllyDeadState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.Animator?.SetTrigger("4_Death");
        ally.PerformDie();
    }

    public void Update(Ally ally) { }

    public void Exit(Ally ally) { }
}