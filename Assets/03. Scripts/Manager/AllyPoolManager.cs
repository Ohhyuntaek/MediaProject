using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    private int _spawnCount = 0;
    // 현재 활성화 된 Ally 리스트
    [FormerlySerializedAs("activateAillies")] public List<GameObject> activateAllies = new();
    
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

    public GameObject SpawnAlly(AllyType type, LineType cardLine)
    {
        AllyTile tile = TileManager.Instance.GetAvailableTile();
        if (tile == null) return null;

        var pool = System.Array.Find(allyPools, p => p.allyType == type);
        if (pool.pool.Count > 0)
        {
            GameObject obj = pool.pool.Dequeue();
            obj.transform.position = tile.transform.position;
            obj.SetActive(true);
            tile.isOccupied = true;

            Ally ally = obj.GetComponent<Ally>();
            ally.Init(tile.transform.position, tile);

            // 발판 강화 조건 확인
            if (tile.lineType == cardLine)
            {
                ally.ApplyTileBonus(); // 강화 효과 부여
            }
            
            activateAllies.Add(obj);
            _spawnCount++;
            return obj;
        }

        return null;
    }

    public void ReturnAlly(AllyType type, GameObject ally)
    {
        // 타일 반환
        Debug.Log("Position: " + ally.transform.position.ToString());
        TileManager.Instance.FreeTile(ally.transform.position);

        // 활성화 된 Ally 리스트에서 제거
        if (activateAllies.Contains(ally))
        {
            activateAllies.Remove(ally);
        }
        
        // Ally 비활성화 후 풀에 다시 넣기
        ally.SetActive(false);
        
        var pool = System.Array.Find(allyPools, p => p.allyType == type);
        pool.pool.Enqueue(ally);
    }
    
    public void PrintActivateAllies()
    {
        // 현재 활성화 되어 있는 Ally의 리스트를 로그에 표시
        foreach (var ally in AllyPoolManager.Instance.activateAllies)
        {
            Debug.Log($"활성 Ally: {ally.name} - 위치: {ally.transform.position}");
        }
    }
    public int SpawnCount
    {
        get => _spawnCount;
        set => _spawnCount = value;
    }
}
