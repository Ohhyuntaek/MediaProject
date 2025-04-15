using UnityEngine;

public class PlayerDeadState : IState<Player>
{
    
    public void Enter(Player player)
    {
        player.Animator?.SetTrigger("4_Death");
       
    }

    public void Update(Player player)
    {
        player.PerformDie();
    }

    public void Exit(Player player)
    {
       
    }
}
