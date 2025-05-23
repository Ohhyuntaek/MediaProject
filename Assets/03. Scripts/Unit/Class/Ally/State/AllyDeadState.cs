using UnityEngine;

public class AllyDeadState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.Animator?.CrossFade("Death", 0f);
        
    }

    public void Update(Ally ally)
    {
        ally.PerformDie();
    }

    public void Exit(Ally ally) { }
}