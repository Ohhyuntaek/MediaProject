using UnityEngine;
using System.Collections.Generic;

public enum DebuffType
{
    DamageAmp,   // 데미지 20% 증가
    Stun,        // 일정 시간 기절
    Slow         // 이동속도 감소
}
public class DebuffSkill : ISkill<Ally> //selemender
{
    public void Activate(Ally owner)
    {
        
        List<Enemy> targets = owner.DetectNearestEnemyTileEnemies(); 

        if (targets.Count == 0)
        {
            return;
        } 
        Debug.Log(targets.Count +" 그 셀에 있는 적의 숫자 ");

        DebuffType selectedDebuff = (DebuffType)owner.GetSkillRandomNum();

        
        foreach (Enemy enemy in targets)
        {
            if (selectedDebuff == DebuffType.DamageAmp)
            {
                enemy.ApplyDefenseBuffDebuff(2f,3f,false);
            }
            else if (selectedDebuff == DebuffType.Slow)
            {
                enemy.ApplySpeedBuffDebuff(2f,3f,false);
            }
            else if(selectedDebuff == DebuffType.Stun)
            {
                Debug.Log(enemy.name);
                enemy.ApplyStun(2f);
            }
            enemy.TakeDamage(10);
        }

        
    }
    


}