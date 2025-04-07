using UnityEngine;

public class EnemyAttackState : IState<Enemy>
{
    private float _attackTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("2_Attack");
        _attackTimer = 1f / enemy.EnemyData.AtkSpeed;
    }

    public void Update(Enemy enemy)
    {
        _attackTimer -= Time.deltaTime;
        
        if (_attackTimer <= 0f) 
        {
            if (enemy.IsTargetInRange()) // 사정거리 안인 경우
            {
                enemy.PerformAttack();
                _attackTimer = 1f / enemy.EnemyData.AtkSpeed;
            }
            else
            {
                enemy.ChangeState(new EnemyWalkState());
            }
        }
        else
        {
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
      
    }
}