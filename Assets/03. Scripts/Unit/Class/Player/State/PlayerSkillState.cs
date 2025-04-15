using UnityEngine;

public class PlayerActiveSkillState : IState<Player>
{
    private bool finished = false;

    public void Enter(Player player)
    {
        player.Animator?.SetTrigger("5_Active");
        
        
    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ActiveSkill") && stateInfo.normalizedTime > 0.9f &&!finished)
        {
            player.PerformActiveSkill();
            finished = true;
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(Player player)
    {
        // 아무 처리 없음
    }
}