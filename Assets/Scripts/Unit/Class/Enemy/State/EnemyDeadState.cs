using UnityEngine;

public class EnemyDeadState : IState<Enemy>
{
    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("4_Death");
        enemy.PerformDie();
    }

    public void Update(Enemy enemy) { }
    public void Exit(Enemy enemy) { }
}
