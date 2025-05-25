using UnityEngine;

public class PlayerDeadState : IState<Dawn>
{
    
    public void Enter(Dawn dawn)
    {
        dawn.Animator?.CrossFade("Death", 0f);
        Debug.Log("죽엇다.");
    }

    public void Update(Dawn dawn)
    {
        dawn.PerformDie();
    }

    public void Exit(Dawn dawn)
    {
       
    }
}
