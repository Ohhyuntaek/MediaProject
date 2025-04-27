using UnityEngine;

public class PlayerIdleState : IState<Dawn>
{
    
    public void Enter(Dawn dawn)
    {
    }

    public void Update(Dawn dawn)
    {
       
        TransionTo(dawn);
        
    }

    public void TransionTo(Dawn dawn)
    {
        if (Input.GetKeyUp(KeyCode.Space) && dawn.CanUseActiveSkill)
        {
            dawn.ChangeState(new PlayerActiveSkillState() );
        }
        
    } 

    public void Exit(Dawn dawn)
    {
       
    }
}
