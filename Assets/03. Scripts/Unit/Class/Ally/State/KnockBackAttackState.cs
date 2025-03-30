using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KnockbackAttackState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.StartCoroutine(AttackRoutine(ally));
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

    public void Update(Ally ally) { }
    public void Exit(Ally ally) { }
}