using UnityEngine;

public class PlayerAttackState : IState<Player>
{
    private float _skillDuration;

    public void Enter(Player player)
    {
        Debug.Log($"[Player:{player.name}] 공격 스킬 발동!");
        player.Animator?.SetTrigger("2_Attack");
        player.PerformSkill();
        _skillDuration = player.UnitData.SkillCoolTime; 
    }

    public void Update(Player player)
    {
        _skillDuration -= Time.deltaTime;

        if (_skillDuration <= 0f)
        {
            player.ChangeState(new PlayerIdleState()); 
        }
    }

    public void Exit(Player player)
    {
        Debug.Log($"[Player:{player.name}] 공격 상태 종료");
    }
}