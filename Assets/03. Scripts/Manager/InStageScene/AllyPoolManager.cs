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

public class AllyPoolManager : MonoBehaviour
{
    /// <summary>
    /// 현재 활성화 된 Ally 리스트
    /// </summary>
    public List<GameObject> activateAllies = new();
    
    [SerializeField] private AllyPool[] allyPools;
    [SerializeField] private int spawnCount = 0;

    [SerializeField] private List<UnitData> unitDataList;
    
    public int SpawnCount
    {
        get => spawnCount;
        set => spawnCount = value;
    }
    
    void Awake()
    {
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

    /// <summary>
    /// Pool에 있는 Ally 스폰
    /// </summary>
    /// <param name="allyType"></param>
    /// <param name="lineType"></param>
    /// <returns></returns>
    public GameObject SpawnAlly(UnitData unitData, LineType lineType)
    {
        AllyTile tile = InGameSceneManager.Instance.tileManager.GetAvailableTile();
        if (tile == null) return null;

        // 풀에서 해당 AllyType의 프리팹 가져오기
        var pool = System.Array.Find(allyPools, p => p.allyType == unitData.AllyType);
        if (pool == null || pool.pool.Count == 0)
        {
            Debug.LogWarning($"AllyType {unitData.AllyType}의 풀에 사용 가능한 오브젝트가 없습니다.");
            return null;
        }

        GameObject obj = pool.pool.Dequeue();
        obj.transform.position = tile.transform.position;
        obj.SetActive(true);
        tile.isOccupied = true;

        // Sorting Layer 설정
        Transform parent = tile.transform.parent;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (parent != null && sr != null)
        {
            if (parent.name == "Up_Line_Tiles") sr.sortingLayerName = "AllysUpLine";
            else if (parent.name == "Down_Line_Tiles") sr.sortingLayerName = "AllysDownLine";
        }

        // Ally 초기화
        Ally ally = obj.GetComponent<Ally>();
        ally.Init(tile.transform.position, tile);
        ally.InitPatternColliders();

        if (tile.lineType == lineType)
            ally.ApplyTileBonus();

        // 코스트 차감
        InGameSceneManager.Instance.costManager.DecreaseCost(unitData.Cost);

        activateAllies.Add(obj);
        spawnCount++;

        return obj;
    }

    /// <summary>
    /// 스폰된 Ally를 다시 Pool로 반환
    /// </summary>
    /// <param name="allyType"></param>
    /// <param name="ally"></param>
    public void ReturnAlly(AllyType allyType, GameObject ally)
    {
        // 타일 반환
        Debug.Log("Position: " + ally.transform.position.ToString());
        InGameSceneManager.Instance.tileManager.FreeTile(ally.transform.position);

        // 활성화 된 Ally 리스트에서 제거
        if (activateAllies.Contains(ally))
        {
            activateAllies.Remove(ally);
        }
        
        // Ally 비활성화 후 풀에 다시 넣기
        ally.SetActive(false);
        
        var pool = System.Array.Find(allyPools, p => p.allyType == allyType);
        pool.pool.Enqueue(ally);
    }

    public List<Ally> GetLineObject_Spawned(LineType lineType)
    {
        List<Ally> allyLineList = new List<Ally>();
        foreach (GameObject allyobject in activateAllies)
        {
            Ally ally = allyobject.GetComponent<Ally>(); 
            AllyTile allytype = ally.OccupiedTile;
            if (allytype.LineType == lineType)
            {
                allyLineList.Add(ally);
            }
            
        }

        return allyLineList;
    }
    
    public void PrintActivateAllies()
    {
        // 현재 활성화 되어 있는 Ally의 리스트를 로그에 표시
        foreach (var ally in activateAllies)
        {
            Debug.Log($"활성 Ally: {ally.name} - 위치: {ally.transform.position}");
        }
    }
    
    public UnitData GetUnitDataByAllyType(AllyType type)
    {
        foreach (var data in unitDataList)
        {
            if (data.AllyType == type)
                return data;
        }

        Debug.LogError($"AllyType {type}에 해당하는 UnitData를 찾을 수 없습니다.");
        return null;
    }
}
