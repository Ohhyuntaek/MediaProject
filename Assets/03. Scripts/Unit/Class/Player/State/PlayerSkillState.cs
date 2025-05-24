using UnityEngine;

public class PlayerActiveSkillState : IState<Dawn>
{
    private bool finished = false;

    public void Enter(Dawn dawn)
    {
        dawn.Animator?.SetTrigger("5_Active");
        dawn.useEnerge();
        EntireGameManager.Instance.soundManager.PlaySfx(dawn.DawnData.SkillSound,dawn.transform.position);
    }

    public void Update(Dawn dawn)
    {
        AnimatorStateInfo stateInfo = dawn.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ActiveSkill") && stateInfo.normalizedTime > 0.9f &&!finished)
        {
            dawn.PerformActiveSkill();
            finished = true;
            dawn.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(Dawn dawn)
    {
        // 아무 처리 없음
    }
}