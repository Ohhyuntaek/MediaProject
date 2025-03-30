using UnityEngine;
using System.Collections;

public enum BuffType
{
    ATKSPEED
}
public class AllyBuffState : IState<Ally>
{
    private int _enemyCount;

    public AllyBuffState(int enemyCount)
    {
        _enemyCount = enemyCount;
    }

    public void Enter(Ally ally)
    {
        ally.StartCoroutine(BuffRoutineJandark(ally));
    }

    private IEnumerator BuffRoutineJandark(Ally ally)
    {
       
        ally.Animator.SetTrigger("3_Buff");
        yield return new WaitForSeconds(0.5f); 
        ally.ApplyBuffByEnemyCount(_enemyCount,BuffType.ATKSPEED );
        ally.ChangeState(new AllyIdleState());
    }

    public void Update(Ally ally) { }
    public void Exit(Ally ally) { }
}