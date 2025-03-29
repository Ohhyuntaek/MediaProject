using UnityEngine;
using System.Collections;

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

        // 유닛에 따라 다음 상태 분기
        switch (ally.UnitData.UnitName)
        {
            case "KnockbackWarrior":
                ally.ChangeState(new KnockbackAttackState());
                break;

            default:
                ally.ChangeState(new AllyIdleState());
                break;
        }
    }

    public void Update(Ally ally) { }

    public void Exit(Ally ally) { }
}