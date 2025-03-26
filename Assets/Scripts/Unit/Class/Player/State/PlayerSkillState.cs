using UnityEngine;

public class PlayerSkillState : IState<Player>
{
    private float _duration;

    public void Enter(Player player)
    {
        player.Animator?.SetTrigger("3_Skill");
        player.PerformSkill(); 
        _duration = 1f; 
    }

    public void Update(Player player)
    {
        _duration -= Time.deltaTime;
        if (_duration <= 0f)
        {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(Player player)
    {
        // 아무 처리 없음
    }
}