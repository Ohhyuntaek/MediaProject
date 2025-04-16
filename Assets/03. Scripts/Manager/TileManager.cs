using System.Collections.Generic;
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
        foreach (var tile in allTiles)
        {
            if (!tile.isOccupied)
            {
                return tile;
            }
        }

        return null;
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

