
using System.Collections.Generic;
using UnityEngine;

public class JandarkPassiveSkill : ISkill<Player>
{
    private int lastcount = 0;
    public void Activate(Player owner)
    {   
        
        if (CheckActivationCondition())
        {   
            int aliveallyCount = AllyPoolManager.Instance.activateAllies.Count;
            Ally spawnAlly = AllyPoolManager.Instance.activateAllies[aliveallyCount - 1].GetComponent<Ally>();
            List<Ally> aroundAllies = spawnAlly.GetAroundAllies();
            Debug.Log(spawnAlly.UnitData.UnitName);
            if (aroundAllies.Count != 0)
            {
                foreach (Ally aroundally in aroundAllies)
                {
                    if (aroundally != null)
                    {
                        aroundally.SetCanCCByDuration(1f);
                    }
                    
                }
               
            }
            
            
        }
    }
    
     
    public bool CheckActivationCondition()
    {
        int count = AllyPoolManager.Instance.SpawnCount;
        if (lastcount != count &&count % 3 == 0 && count != 0)
        {
            lastcount = count;
            return true;
        }
        else
        {
            return false;
        }
    }
}
