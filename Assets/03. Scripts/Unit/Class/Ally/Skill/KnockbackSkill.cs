using System.Collections.Generic;
using UnityEngine;


public class KnockbackSkill : ISkill<Ally>
{
    public void Activate(Ally ally)
    {
        List<Enemy> targets = ally.DetectTargets(ally.UnitData.AttackRange);

        if (targets.Count == 0)
        {
            Debug.Log("넉백 대상 없음");
            return;
        }

        
        ally.ApplyKnockback(targets);

        
        ally.SetLastKnockbackEnemyCount(targets.Count);
    }
    

}