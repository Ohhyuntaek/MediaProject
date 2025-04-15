
using UnityEngine;

public class Basic3 : ISkill<Enemy>
{
    public void Activate(Enemy owner)
    {
        Vector3 spawnPosition = owner.transform.GetChild(0).localPosition;
        Vector3 spawnWorldPostion = owner.transform.TransformPoint(spawnPosition);
        owner.SpawAttackEffect(spawnWorldPostion,0);
        
    }
}
