
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NightLordSkill : ISkill<Ally>
{
    
    public void Activate(Ally owner)
    {
        //1.적탐지
        List<Enemy> detectList = owner.DetectTargets(3);
        //2.데미지 적용
        owner.ApllyDamageMulti(detectList);
        //3. 조건검사
        if (owner.GetTotalDamage >= 30 && owner.GetTotalLifeTime<12f)
        {
            owner.SetLifetime(1.5f);
        }
        
        

    }
}
