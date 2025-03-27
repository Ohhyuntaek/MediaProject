using UnityEngine;

public class AllyIdleState : IState<Ally>
{
    private float _attackTimer;

    public void Enter(Ally ally)
    {
        _attackTimer = 1f / ally.UnitData.AttackSpeed;
        ally.Animator?.SetBool("1_Move", false);
    }

    public void Update(Ally ally)
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            ally.ChangeState(new AllyAttackState());
        }
    }

    public void Exit(Ally ally)
    {
    
    }
}