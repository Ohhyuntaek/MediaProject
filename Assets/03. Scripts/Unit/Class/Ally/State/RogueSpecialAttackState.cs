
using System.Collections.Generic;
using UnityEngine;

public class RogueSpecialAttackState : IState<Ally>
{
    private bool _final = false;
    private List<IDamageable> _detectlist;
    public void Enter(Ally ally)
    {
        ally.Animator.SetTrigger("5_SpecialAttack");
       

    }

    public void Update(Ally ally)
    {
        TranstionTo(ally);
    }

    public void TranstionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SpecialAttack") && !_final && stateInfo.normalizedTime > 0.9f)
        {
            _final = true;
            _detectlist = ally.DetectTargets();
            ally.GrabEnemies(_detectlist);
            
            ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
        }
    }

    public void Exit(Ally ally)
    {
        
    }

    public void GrabEnemy()
    {
        
    }
}
