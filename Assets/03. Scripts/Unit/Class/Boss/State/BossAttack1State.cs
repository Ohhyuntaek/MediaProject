
using UnityEngine;

public class BossAttack1State : IState<Boss>
{
    private bool _finished = false;
    public void Enter(Boss boss)
    {
        boss.Animator.SetTrigger("2_Attack1");
        EntireGameManager.Instance.soundManager.PlaySfx(boss.BossData.AttackSound[0],boss.transform.position,false);
    }

    public void Update(Boss owner)
    {
       TransionTo(owner);
    }

    public void TransionTo(Boss boss)
    {
        AnimatorStateInfo stateInfo = boss.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack1") && stateInfo.normalizedTime > 0.9f && !_finished )
        {   
            
            _finished = true;
            Ally closeAlly = boss.GetClosestAlly(boss.transform.position);
            if (closeAlly != null)
            {
                closeAlly.ChangeState(new AllyDeadState());
            }
            else
            {
                boss.DealDamageToPlayer(10);
            }
            boss.ChangeState(new BossIdleState());
        }
    }
    public void Exit(Boss boss)
    {
        boss.InitializeAttack();
        boss.LastSpecialAttack = false;
    }
}
