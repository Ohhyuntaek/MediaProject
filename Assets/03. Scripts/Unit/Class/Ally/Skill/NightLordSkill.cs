
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NightLordSkill : ISkill<Ally>
{
    
    public void Activate(Ally owner)
    {
        //1.적탐지
        List<IDamageable> detectList = owner.DetectTargets();
        owner.ApllyDamageMulti(detectList);
        Debug.Log(owner.GetTotalDamage);
        //3. 조건검사
        if (owner.GetTotalDamage >= 30 && owner.GetTotalLifeTime<12f)
        {
            owner.SetLifetime(1.5f);
        }
        
        

    }
}
