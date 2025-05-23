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
    private bool _onece = false;
    public void Activate(Ally owner)
    {
        // 근접한 타일의 적/보스들을 모두 감지
        List<IDamageable> targets = owner.DetectNearestTileTargets(); 
        if (targets.Count == 0)
            return;

        Debug.Log($"{targets.Count} 그 셀에 있는 대상의 숫자");

        
        DebuffType selectedDebuff = (DebuffType)owner.GetSkillRandomNum();

        foreach (var t in targets)
        {
            if (t is Enemy enemy)
            {
               
                switch (selectedDebuff)
                {
                    case DebuffType.DamageAmp:
                        enemy.ApplyDefensDebuff();
                        ParticleManager.Instance.PlaySkillParticle(AllyType.Salamander,enemy.transform.position,0); //파이어볼
                        break;
                    case DebuffType.Slow://아이스볼
                        enemy.ApplySpeedBuffDebuff(2f, 3f, false);
                        if (!_onece)
                        {
                            _onece = true;
                            ParticleManager.Instance.PlaySkillParticle(AllyType.Salamander,enemy.transform.position,2);
                        }
                        break;
                    case DebuffType.Stun: //썬더
                        enemy.ApplyStun(2f);
                        if (!_onece)
                        {
                            _onece = true;
                            ParticleManager.Instance.PlaySkillParticle(AllyType.Salamander,enemy.transform.position,1);
                        }
                        break;
                }
                enemy.TakeDamage(10);
                
               
               
            }
            else if (t is Boss boss)
            {
                // 보스에게는 CC만 적용
                boss.ApplyCC();
                boss.TakeDamage(10);
                ParticleManager.Instance.PlaySkillParticle(AllyType.Salamander,boss.transform.position,(int)selectedDebuff);
            }
        }
        
    }
    


}