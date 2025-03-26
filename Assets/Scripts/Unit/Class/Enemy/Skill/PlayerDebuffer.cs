using UnityEngine;

public class PlayerDebuffer : ISkill
{
    public void Activate(MonoBehaviour caster)
    {
        Debug.Log($"[{caster.name}] 플레이어 방해 스킬 발동!");
    }
}
