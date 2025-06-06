using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class KnockbackAttackState : IState<Ally>
{
    private bool finished = false;
    public void Enter(Ally ally)
    {
       
        ally.Animator.SetTrigger("2_1KnockBack");
        Debug.Log("넉백 스테이트");
        EntireGameManager.Instance.soundManager.PlaySfx(ally.UnitData.AttackSound[1],ally.transform.position);
    }

    

    public void Update(Ally ally)
    {
        TransitionTo(ally);
    }

    private void TransitionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("KnockBackAttack") && !finished && stateInfo.normalizedTime > 0.9f)
        {
            string _unitName = ally.UnitData.UnitName;
            finished = true;
            ally.PerformSkill();
            
            
            switch (_unitName)
            {
                case "Jandark":
                    
                    EntireGameManager.Instance.soundManager.PlaySfx(ally.UnitData.AttackSound[1],ally.transform.position);
                    if (ally.OnTile)
                    { 
                        ally.ChangeState(new AllyBuffState(ally.GetLastKnockbackEnemyCount()));
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    }
                    break;
                default:
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
           
            }
        }
    }

    public void Exit(Ally ally) { }
}