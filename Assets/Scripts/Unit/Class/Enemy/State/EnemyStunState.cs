using UnityEngine;

public class EnemyStunState : IState<Enemy>
{
    private float _stunTimer;

    public EnemyStunState(float duration)
    {
        _stunTimer = duration;
    }

    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("Stun");
    }

    public void Update(Enemy enemy)
    {
        _stunTimer -= Time.deltaTime;
        if (_stunTimer <= 0f)
        {
            enemy.ChangeState(new EnemyWalkState());
        }
    }

    public void Exit(Enemy enemy) { }
}