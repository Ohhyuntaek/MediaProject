using UnityEngine;

public class AllyIdleState : IState<Ally>
{
    private float _attackTimer;

    public void Enter(Ally ally)
    {
        _attackTimer = 1f / ally.UnitData.AttackSpeed;
        ally.Animator?.SetBool("1_Move", false);
        //Debug.Log("아이들상태 진입");
    }

    public void Update(Ally ally)
    {
        //Debug.Log("check");
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            ally.ChangeState(new AllyAttackState());
        }
    }

    public void Exit(Ally ally) { }
}