using UnityEngine;
using System.Collections.Generic;

public enum DebuffType
{
    DamageAmp,   // 데미지 20% 증가
    Stun,        // 일정 시간 기절
    Slow         // 이동속도 감소
}
public class SalamenderSkill : ISkill<Ally> //selemender
{
   
    public void Activate(Ally owner)
    {
        // 근접한 타일의 적/보스들을 모두 감지
        List<IDamageable> targets = owner.DetectNearestTileTargets(); 
        if (targets.Count == 0)
            return;

        Debug.Log($"{targets.Count} 그 셀에 있는 대상의 숫자");

        // 0 또는 1 또는 2 중 랜덤, 0=DamageAmp, 1=Slow, 2=Stun
        DebuffType selectedDebuff = (DebuffType)owner.GetSkillRandomNum();

        foreach (var t in targets)
        {
            if (t is Enemy enemy)
            {
                // 적 유닛에게만 원래 디버프 & 데미지 로직
                switch (selectedDebuff)
                {
                    case DebuffType.DamageAmp:
                        enemy.ApplyDefenseBuffDebuff(2f, 3f, false);
                        break;
                    case DebuffType.Slow:
                        enemy.ApplySpeedBuffDebuff(2f, 3f, false);
                        break;
                    case DebuffType.Stun:
                        enemy.ApplyStun(2f);
                        break;
                }
                enemy.TakeDamage(10);
            }
            else if (t is Boss boss)
            {
                // 보스에게는 CC만 적용
                boss.ApplyCC();
                boss.TakeDamage(10);
            }
        }
    }
    


}