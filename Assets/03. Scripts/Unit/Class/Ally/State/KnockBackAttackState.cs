using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;

public class KnockbackAttackState : IState<Ally>
{
    private bool finished = false;
    public void Enter(Ally ally)
    {
        //ally.StartCoroutine(AttackRoutine(ally));
        ally.Animator.SetTrigger("2_1KnockBack");
        Debug.Log("넉백 스테이트");
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
                    
                    SoundManager.Instance.PlaySfx(ally.UnitData.AttackSound[1],ally.transform.position);
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