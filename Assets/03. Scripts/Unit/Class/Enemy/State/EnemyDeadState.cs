using UnityEngine;

public class EnemyDeadState : IState<Enemy>
{
    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("4_Death");
        
        
    }

    public void Update(Enemy enemy)
    {
        enemy.PerformDie();
    }
    public void Exit(Enemy enemy) { }
}
