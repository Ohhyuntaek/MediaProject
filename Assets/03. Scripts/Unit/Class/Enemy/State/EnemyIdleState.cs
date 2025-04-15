using UnityEngine;

public class EnemyIdleState : IState<Enemy>
{
    private float _attackTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetBool("1_Move", false); 
        _attackTimer = 1f / enemy.EnemyData.MoveSpeed;
    }

    public void Update(Enemy enemy)
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f) 
        {   
            if (enemy.IsTargetInRange())
            {
                enemy.ChangeState(new EnemyAttackState());
            }
            else
            {
                enemy.ChangeState(new EnemyWalkState());
            }
        }
        
    }

    public void Exit(Enemy enemy)
    {
      
    }
}