using System.Collections.Generic;
using UnityEngine;

public class JanDarkPlayerActiveSkill : ISkill<Dawn>
{
    public void Activate(Dawn owner)
    {
        List<GameObject> allyList = AllyPoolManager.Instance.activateAllies;
        foreach (GameObject ally  in allyList)
        {
            if (ally.GetComponent<Ally>().Dead)
            {
                continue;
            }
            else
            {
                ally.GetComponent<Ally>().SetLifetime(5f);
            }
            
        }
    }
}
