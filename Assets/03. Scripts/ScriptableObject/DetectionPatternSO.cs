using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="SO/Detection Pattern")]
public class DetectionPatternSO : ScriptableObject
{
   [SerializeField]public List<Vector2Int> cellOffsets = new List<Vector2Int>();
   
}
