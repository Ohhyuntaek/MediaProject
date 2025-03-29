using UnityEngine;
using System.Collections;

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

        // 🧠 PerformAttack 내부에서 넉백 + 카운트 → ally에 저장
        ally.PerformAttack();

        // ⛓ 상태 전이
        if (ally.UnitData.UnitName == "KnockbackWarrior")
        {
            // 넉백 대상 수를 ally가 기억하고 있음
            ally.ChangeState(new AllyBuffState(ally.GetLastKnockbackEnemyCount()));
        }
        else
        {
            ally.ChangeState(new AllyIdleState());
        }
    }

    public void Update(Ally ally) { }
    public void Exit(Ally ally) { }
}