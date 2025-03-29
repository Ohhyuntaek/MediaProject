using UnityEngine;
using System.Collections;

public class AllyBuffState : IState<Ally>
{
    private int _enemyCount;

    public AllyBuffState(int enemyCount)
    {
        _enemyCount = enemyCount;
    }

    public void Enter(Ally ally)
    {
        ally.StartCoroutine(BuffRoutine(ally));
    }

    private IEnumerator BuffRoutine(Ally ally)
    {
      
        ally.Animator.SetTrigger("3_Buff");
        yield return new WaitForSeconds(0.5f); 

        
        ally.ApplyAllyAttackSpeedBuff(_enemyCount);

      
        ally.ChangeState(new AllyIdleState());
    }

    public void Update(Ally ally) { }
    public void Exit(Ally ally) { }
}