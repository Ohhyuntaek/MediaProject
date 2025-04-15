using UnityEngine;

public class PlayerIdleState : IState<Player>
{
    
    public void Enter(Player player)
    {
    }

    public void Update(Player player)
    {
       
        TransionTo(player);
        
    }

    public void TransionTo(Player player)
    {
        if (Input.GetKeyUp(KeyCode.Space) && player.CanUseActiveSkill)
        {
            player.ChangeState(new PlayerActiveSkillState() );
        }
        
    } 

    public void Exit(Player player)
    {
       
    }
}
