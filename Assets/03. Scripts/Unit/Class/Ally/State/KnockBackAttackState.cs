using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KnockbackAttackState : IState<Ally>
{
    private bool finished = false;
    public void Enter(Ally ally)
    {
        //ally.StartCoroutine(AttackRoutine(ally));
        ally.Animator.SetTrigger("2_1KnockBack");
    }

    private IEnumerator AttackRoutine(Ally ally)
    {
        ally.Animator.SetTrigger("2_1KnockBack");
        yield return new WaitForSeconds(1f / ally.UnitData.AttackSpeed);

        
        ally.PerformAttack();
        string _unitName = ally.UnitData.UnitName;

        switch (_unitName)
        {
            case "KnockbackWarrior":
                if (ally.OnTile)
                { 
                    ally.ChangeState(new AllyBuffState(ally.GetLastKnockbackEnemyCount()));
                }
                else
                {
                    ally.ChangeState(new AllyIdleState());
                }
                break;
           default:
                ally.ChangeState(new AllyIdleState());
                break;
           
        }
      
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
            switch (_unitName)
            {
                case "KnockbackWarrior":
                    if (ally.OnTile)
                    { 
                        ally.ChangeState(new AllyBuffState(ally.GetLastKnockbackEnemyCount()));
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState());
                    }
                    break;
                default:
                    ally.ChangeState(new AllyIdleState());
                    break;
           
            }
        }
    }

    public void Exit(Ally ally) { }
}