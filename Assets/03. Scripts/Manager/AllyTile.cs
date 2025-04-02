using UnityEngine;

public enum LineType { Front, Mid, Rear }

public class AllyTile : MonoBehaviour
{
    public LineType lineType;
    public bool isOccupied = false;
}
