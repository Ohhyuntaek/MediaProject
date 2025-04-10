

using UnityEngine;

public class AllyReviveState :IState<Ally>
{
    private bool finished = false;
    public void Enter(Ally owner)
    {
       owner.Animator.SetTrigger("4_Revive");
       
    }

    public void Update(Ally owner)
    {
       TransitionTo(owner);
    }

    public void Exit(Ally owner)
    {
       
    }

    private void TransitionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("OnStun") && !finished && stateInfo.normalizedTime > 0.9f)
        {
            string _unitName = ally.UnitData.UnitName;
            finished = true;
            switch (_unitName)
            {
                case "NightLord":
                        Debug.Log("부활 후 idle 상태 진입");
                        ally.SetBaseAttack(5);
                        ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    
                    break;
                default:
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
           
            }
        }
    }
}
