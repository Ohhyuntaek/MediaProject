using UnityEngine;

public class AllySupporter : ISkill<Enemy>
{
    public void Activate(Enemy caster)
    {
        Debug.Log($"[{caster.name}]  적군 버프 스킬 발동!");
    }
}
