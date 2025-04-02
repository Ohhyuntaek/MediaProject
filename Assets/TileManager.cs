using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private List<AllyTile> frontTiles = new();
    private List<AllyTile> midTiles = new();
    private List<AllyTile> rearTiles = new();

    void Awake()
    {
        Instance = this;

        foreach (AllyTile tile in FindObjectsOfType<AllyTile>())
        {
            switch (tile.lineType)
            {
                case LineType.Front:
                    frontTiles.Add(tile);
                    break;
                case LineType.Mid:
                    midTiles.Add(tile);
                    break;
                case LineType.Rear:
                    rearTiles.Add(tile);
                    break;
            }
        }
    }

    public Vector3 GetAvailableTilePosition(LineType line)
    {
        List<AllyTile> tileList = line switch
        {
            LineType.Front => frontTiles,
            LineType.Mid => midTiles,
            LineType.Rear => rearTiles,
            _ => null
        };

        foreach (var tile in tileList)
        {
            if (!tile.isOccupied)
            {
                tile.isOccupied = true;
                return tile.transform.position;
            }
        }

        return Vector3.zero;
    }

    public void FreeTile(Vector3 position)
    {
        foreach (AllyTile tile in FindObjectsOfType<AllyTile>())
        {
            if (tile.transform.position == position)
            {
                tile.isOccupied = false;
                break;
            }
        }
    }
}
