using UnityEngine;

public class EnemyDeadState : IState<Enemy>
{
    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("4_Death");
        SoundManager.Instance.PlaySfx(enemy.EnemyData.DeathSound[0],enemy.transform.position,false);
        
    }

    public void Update(Enemy enemy)
    {
        enemy.PerformDie();
    }
    public void Exit(Enemy enemy) { }
}
