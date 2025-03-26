using UnityEngine;

public class EnemyAttackState : IState<Enemy>
{
    private float _attackTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("2_Attack");
        _attackTimer = 1f / enemy.UnitData.AttackSpeed;
    }

    public void Update(Enemy enemy)
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            if (enemy.IsTargetInRange())
            {
                enemy.PerformAttack();
                _attackTimer = 1f / enemy.UnitData.AttackSpeed;
            }
            else
            {
                enemy.ChangeState(new EnemyWalkState());
            }
        }
    }

    public void Exit(Enemy enemy) { }
}