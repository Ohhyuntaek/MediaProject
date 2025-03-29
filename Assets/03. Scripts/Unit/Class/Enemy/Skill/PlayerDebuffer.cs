using UnityEngine;

public class PlayerDebuffer : ISkill<Enemy>
{
    public void Activate(Enemy caster)
    {
        Debug.Log($"[{caster.name}] 플레이어 방해 스킬 발동!");
    }
}
