
using UnityEngine;

public class BossDeadState : IState<Boss>
{
    private bool _finished = false;
    public void Enter(Boss boss)
    {
        boss.Animator.SetTrigger("4_Death");
        SoundManager.Instance.PlaySfx(boss.BossData.DeathSound[0],boss.transform.position,false);
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
        
    }
}
