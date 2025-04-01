using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class AllyAttackState : IState<Ally>
{
    public void Enter(Ally ally)
    {
        ally.StartCoroutine(AttackRoutine(ally));
    }

    private IEnumerator AttackRoutine(Ally ally)
    {
        
        ally.Animator.SetTrigger("2_Attack");

    
        yield return new WaitForSeconds(1f / ally.UnitData.AttackSpeed);

        
        ally.PerformAttack();

        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        switch (ally.UnitData.UnitName)
        {
            case "KnockbackWarrior":
                ally.ChangeState(new KnockbackAttackState());
                break;

            case "Slamander":
                if (!ally.FinalSkill)
                {
                    
                    ally.ChangeState(new AllyDebuffAttackState());
                    ally.SetFinalSkill(true);
                }
                else
                {
                    ally.ChangeState(new AllyIdleState());
                }
                break;
            case "BountyHunter" :
                //if(발판을 밝고 있을 시))//TODO : 범위공격
                //else(발판 미적용 시) //TODO : 단일공격
                //TODO : 바운티 헌터 구현 
                break;

            default:
                ally.ChangeState(new AllyIdleState());
                break;
        }
    }

    public void Update(Ally ally) { }

    public void Exit(Ally ally) { }
}