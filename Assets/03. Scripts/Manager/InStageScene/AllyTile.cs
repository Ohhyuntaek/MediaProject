using System;
using System.Collections.Generic;
using UnityEngine;

public enum LineType { Front, Mid, Rear }

public class AllyTile : MonoBehaviour
{
    public LineType lineType;
    public bool isOccupied = false;
    public bool dir;//false면 up true면 down
    public Ally ally;
    public PolygonCollider2D _hitCollider;

    private void Start()
    {
        isOccupied = false;
    }

    private void Update()
    {
        
    }

    public List<Ally> GetAroundAlly()
    {
        List<Ally> allies = new List<Ally>();
        int digitIndex = gameObject.name.Length - 1;
        int currenttileNum = int.Parse(gameObject.name[gameObject.name.Length - 1].ToString());
        
        if (!dir)
        {
            for (int i = -1; i <= 1; i+=2)
            {   
                int aroundIndex = currenttileNum + i;
                if (aroundIndex <= 6 && aroundIndex >= 1)
                {
                    string aroundString = "AllySpawnPoint_U" + (char)('0' + aroundIndex);
                    Debug.Log(aroundString);
                    Ally findAlly = GameObject.Find(aroundString).GetComponent<AllyTile>().ally;
                    if (findAlly != null)
                    {   
                        allies.Add(findAlly);
                    }
                }
            }
        }
        else
        {   
            for (int i = -1; i <= 1; i+=2)
            {   
                int aroundIndex = currenttileNum + i;
                if (aroundIndex <= 6 && aroundIndex >= 1)
                {
                    string aroundString = "AllySpawnPoint_D"+ (char)('0' + aroundIndex);
                    Debug.Log(aroundString);
                    Ally findAlly = GameObject.Find(aroundString).GetComponent<AllyTile>().ally;
                    if (findAlly != null)
                    {
                        allies.Add(findAlly);
                    }
                }
            }
        }
        
        Debug.Log(allies.Count + " ally 카운트");
        return allies;
    }

    public LineType LineType => lineType;
}
