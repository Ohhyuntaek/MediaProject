using System.Collections.Generic;
using UnityEngine;

public class BossAttack2State : IState<Boss>
{
    private bool _finished = false;
    public void Enter(Boss owner)
    {
        owner.Animator.SetTrigger("2_Attack2");
    }

    public void Update(Boss owner)
    {
            TransionTo(owner);
    }

    public void TransionTo(Boss boss)
    {
        AnimatorStateInfo stateInfo = boss.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack2") && stateInfo.normalizedTime > 0.9f && !_finished)
        {
            _finished = true;
            List<Ally> list = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Rear);
            if (list.Count > 0)
            {
                list[0].ChangeState(new AllyDeadState());
            }
            else
            {
                boss.DealDamageToPlayer(5);
            }
            boss.InitializeAttack();
            boss.ChangeState(new BossIdleState());
        }
    }
    public void Exit(Boss owner)
    {
        
    }
}
