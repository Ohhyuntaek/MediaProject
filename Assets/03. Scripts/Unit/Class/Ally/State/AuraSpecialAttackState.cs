
using UnityEngine;
using UnityEngine.UI;

public class AuraSpecialAttackState : IState<Ally>
{
    private bool _finished = false;
    public void Enter(Ally owner)
    {
       owner.Animator.SetTrigger("5_SpecialAttack");
       owner.PerformSkill();
    }

    public void Update(Ally owner)
    {
       
    }

    public void Trantistion(Ally owner)
    {
        AnimatorStateInfo stateInfo = owner.Animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Teleport") && !_finished && stateInfo.normalizedTime > 0.9f)
        {
            owner.PerformSkill();
            owner.ChangeState(new AllyIdleState(1/owner.ATKSPD));
           
        }
    }

    public void Exit(Ally owner)
    {
       
    }
}
