public class EnemyWalkState : IState<Enemy>
{
    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetBool("1_Move", true);
    }

    public void Update(Enemy enemy)
    {
        enemy.MoveForward();

        if (enemy.IsTargetInRange())
        {
            enemy.ChangeState(new EnemyAttackState());
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.Animator?.SetBool("1_Move", false);
    }
}