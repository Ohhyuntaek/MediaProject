using UnityEngine;

public class AllyDisruptor : ISkill
{
    public void Activate(MonoBehaviour caster)
    {
        Debug.Log($"[{caster.name}] Ally 방해 스킬 발동!");
    }
}
