using UnityEngine;
using System.Collections;

public enum BuffType
{
    ATKSPEED
}
public class AllyBuffState : IState<Ally>
{
    private int _enemyCount;
    private bool finished = false;
    public AllyBuffState(int enemyCount)
    {
        _enemyCount = enemyCount;
    }

    public void Enter(Ally ally)
    {
        //ally.StartCoroutine(BuffRoutineJandark(ally));
        ally.Animator.SetTrigger("3_Buff");
    }

    private IEnumerator BuffRoutineJandark(Ally ally)
    {
       
        ally.Animator.SetTrigger("3_Buff");
        yield return new WaitForSeconds(0.5f); 
        ally.ApplyBuffByEnemyCount(_enemyCount,BuffType.ATKSPEED );
        ally.ChangeState(new AllyIdleState(1/ally.UnitData.AttackSpeed));
    }

    public void Update(Ally ally)
    {
        TranstionTo(ally);
    }

    private void TranstionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Buff") && !finished && stateInfo.normalizedTime > 0.9f)
        {
            ally.ApplyBuffByEnemyCount(_enemyCount,BuffType.ATKSPEED );
            ally.ChangeState(new AllyIdleState(1/ally.UnitData.AttackSpeed));
        }
    }

    public void Exit(Ally ally) { }
}