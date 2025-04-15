using System.Collections.Generic;
using UnityEngine;

public class JanDarkPlayerActiveSkill : ISkill<Player>
{
    public void Activate(Player owner)
    {
        List<GameObject> allyList = AllyPoolManager.Instance.activateAllies;
        foreach (GameObject ally  in allyList)
        {
            ally.GetComponent<Ally>().SetLifetime(5f);
        }
    }
}
