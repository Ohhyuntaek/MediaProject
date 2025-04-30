using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private List<AllyTile> allTiles = new();

    void Awake()
    {
        Instance = this;

        foreach (AllyTile tile in FindObjectsOfType<AllyTile>())
        {
            allTiles.Add(tile);
        }
    }

    // 모든 타일 중 하나 반환
    public AllyTile GetAvailableTile()
    {
        /*
        // 모든 타일을 순서대로 반환
        foreach (var tile in allTiles)
        {
            if (!tile.isOccupied)
            {
                return tile;
            }
        }

        return null;
        */
        
        // 1. 비어 있는 타일만 리스트로 필터링
        List<AllyTile> availableTiles = allTiles.Where(tile => !tile.isOccupied).ToList();

        // 2. 비어 있는 게 없으면 null 반환
        if (availableTiles.Count == 0)
            return null;

        // 3. 랜덤 인덱스로 하나 반환
        int randomIndex = Random.Range(0, availableTiles.Count);
        return availableTiles[randomIndex];
    }

    public void FreeTile(Vector3 position)
    {
        foreach (AllyTile tile in allTiles)
        {
            if (tile.transform.position == position)
            {   
                tile.isOccupied = false;
                tile.ally = null;
                break;
            }
        }
    }
}

