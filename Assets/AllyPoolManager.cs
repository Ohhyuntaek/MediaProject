using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllyPool
{
    public AllyType allyType;
    public GameObject prefab;
    public int poolSize = 5;
    [HideInInspector] public Queue<GameObject> pool = new();
}

public enum AllyType
{
    JoanDarc, NightLord, BountyHunter, Rogue, CentaurLady, Salamander
}

public class AllyPoolManager : MonoBehaviour
{
    public static AllyPoolManager Instance;
    public AllyPool[] allyPools;

    void Awake()
    {
        Instance = this;

        foreach (var pool in allyPools)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                pool.pool.Enqueue(obj);
            }
        }
    }

    public GameObject SpawnAlly(AllyType type, LineType line)
    {
        Vector3 spawnPos = TileManager.Instance.GetAvailableTilePosition(line);
        if (spawnPos == Vector3.zero)
            return null;
        
        var pool = System.Array.Find(allyPools, p => p.allyType == type);
        if (pool.pool.Count > 0)
        {
            GameObject obj = pool.pool.Dequeue();
            obj.transform.position = spawnPos;
            obj.SetActive(true);
            return obj;
        }

        return null;
    }

    public void ReturnAlly(AllyType type, GameObject ally)
    {
        ally.SetActive(false);
        TileManager.Instance.FreeTile(ally.transform.position);
        
        var pool = System.Array.Find(allyPools, p => p.allyType == type);
        pool.pool.Enqueue(ally);
    }
}
