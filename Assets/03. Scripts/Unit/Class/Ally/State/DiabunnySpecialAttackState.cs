
using System.Collections.Generic;
using UnityEngine;

public class DiabunnySpecialAttackState : IState<Ally>
{
    private bool _finished = false;
    public void Enter(Ally ally)
    {
        ally.Animator.SetTrigger("5_SpecialAttack");
    }

    public void Update(Ally ally)
    {
        AnimatorStateInfo state = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("SpecialAttack") && state.normalizedTime >= 0.9f && !_finished)
        {
            _finished = true;
            ally.PerformSkill();
            // 공격 후 Idle로 복귀
            ally.ChangeState(new AllyIdleState(2f));
        }
    }

    public void Exit(Ally ally)
    {
        
    }

    
    
}
