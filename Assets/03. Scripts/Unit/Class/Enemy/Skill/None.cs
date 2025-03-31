using UnityEngine;

public class None : ISkill<Enemy>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Activate(Enemy caster)
    {
        Debug.Log($"[{caster.name}] 그냥 평타 발동!");
    }
}
