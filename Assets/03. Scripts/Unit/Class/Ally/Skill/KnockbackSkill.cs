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

        // 넉백 효과
        ally.ApplyKnockback(targets);

        // 👇 ally에 저장 (다음 상태에서 쓰기 위함)
        ally.SetLastKnockbackEnemyCount(targets.Count);
    }

}