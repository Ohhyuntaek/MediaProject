using System.Collections.Generic;
using UnityEngine;


public class JandarkSkill : ISkill<Ally>
{
    public void Activate(Ally ally)
    {
        List<IDamageable> targets = ally.DetectTargets(ally.UnitData.AttackRange);

        if (targets.Count == 0)
        {
            Debug.Log("넉백 대상 없음");
            
        }
        else
        {
            Debug.Log("넉백적용");
            ally.ApplyKnockback(targets);
            ally.SetLastKnockbackEnemyCount(targets.Count);
        }
        
    }
    

}