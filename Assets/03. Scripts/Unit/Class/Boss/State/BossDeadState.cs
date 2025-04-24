
using UnityEngine;

public class BossDeadState : IState<Boss>
{
    private bool _finished = false;
    public void Enter(Boss owner)
    {
        owner.Animator.SetTrigger("4_Death");
    }

    public void Update(Boss owner)
    {
        AnimatorStateInfo stateInfo = owner.Animator.GetCurrentAnimatorStateInfo(0);
        if (!_finished && stateInfo.IsName("Death") && stateInfo.normalizedTime > 0.9f)
        {
            GameObject.Destroy(owner.gameObject);
        }
        
    }

    public void Exit(Boss owner)
    {
        throw new System.NotImplementedException();
    }
}
