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
        enemy.Animator?.SetBool("5_Stun",true);
    }

    public void Update(Enemy enemy)
    {
        _stunTimer -= Time.deltaTime;
        if (_stunTimer <= 0f)
        {
            enemy.Animator?.SetBool("5_Stun", false);
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.Animator?.SetBool("5_Stun",false);
    }
}