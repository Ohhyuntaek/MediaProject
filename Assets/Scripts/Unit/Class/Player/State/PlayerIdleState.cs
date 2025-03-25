using UnityEngine;

public class PlayerIdleState : IState<Player>
{
    private float _attackTimer;
    public void Enter(Player player)
    {
        _attackTimer = 1f / player.UnitData.AttackSpeed;
        player.Animator?.SetBool("1_Move", false);
    }

    public void Update(Player player)
    {
        //테스트용
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            player.ChangeState(new PlayerAttackState());
        }
    }

    public void Exit(Player player)
    {
       //나중에 구현예정
    }
}
