using UnityEngine;

public class PlayerDeadState : IState<Dawn>
{
    
    public void Enter(Dawn dawn)
    {
        dawn.Animator?.SetTrigger("4_Death");
       
    }

    public void Update(Dawn dawn)
    {
        dawn.PerformDie();
    }

    public void Exit(Dawn dawn)
    {
       
    }
}
