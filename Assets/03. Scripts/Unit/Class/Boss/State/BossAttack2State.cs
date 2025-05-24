using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossAttack2State : IState<Boss>
{
    private bool _finished = false;
    public void Enter(Boss boss)
    {
        boss.Animator.SetTrigger("2_Attack2");
        EntireGameManager.Instance.soundManager.PlaySfx(boss.BossData.AttackSound[1],boss.transform.position,false);
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
            //SoundManager.Instance.PlaySfx(boss.BossData.AttackSound,boss.transform.position,false);
            _finished = true;
            List<Ally> list = InGameSceneManager.Instance.allyPoolManager.GetLineObject_Spawned(LineType.Rear);
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
        owner.LastSpecialAttack = false;
    }
}
