using UnityEngine;

public class PlayerDeadState : IState<Player>
{
    
    public void Enter(Player player)
    {
        player.Animator?.SetTrigger("4_Death");
        player.PerformDie();
    }

    public void Update(Player player)
    {
      
    }

    public void Exit(Player player)
    {
       
    }
}
