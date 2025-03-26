using UnityEngine;

public class AllySupporter : ISkill
{
    public void Activate(MonoBehaviour caster)
    {
        Debug.Log($"[{caster.name}]  적군 버프 스킬 발동!");
    }
}
