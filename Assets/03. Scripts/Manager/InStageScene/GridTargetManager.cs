using System;
using System.Collections.Generic;
using UnityEngine;

public class GridTargetManager : MonoBehaviour
{
    public static GridTargetManager Instance { get; private set; }

    [Serializable]
    public class _2dArray
    {
        public PolygonCollider2D[] arr_row;
    }

    [Header("0: 위행, 1: 아래행")]
    public _2dArray[] coliderMat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LogAllColliders();
    }

    /// <summary>
    /// 씬이 시작될 때, coliderMat 배열 각 행·열에 할당된 PolygonCollider2D 이름을 출력합니다.
    /// </summary>
    private void LogAllColliders()
    {
        for (int row = 0; row < coliderMat.Length; row++)
        {
            var rowArr = coliderMat[row].arr_row;
            if (rowArr == null)
            {
                Debug.LogWarning($"[GridTargetManager] row {row} 에 arr_row가 null 입니다.");
                continue;
            }

            for (int col = 0; col < rowArr.Length; col++)
            {
                var poly = rowArr[col];
                if (poly != null)
                    Debug.Log($"[GridTargetManager] [{row}, {col}] = {poly.name}");
                else
                    Debug.Log($"[GridTargetManager] [{row}, {col}] = (없음)");
            }
        }
    }

    public bool TryGetGridIndex(PolygonCollider2D targetCollider, out int row, out int col)
    {
        for (int r = 0; r < coliderMat.Length; r++)
        {
            var rowArr = coliderMat[r].arr_row;
            if (rowArr == null) continue;

            for (int c = 0; c < rowArr.Length; c++)
            {
                if (rowArr[c] == targetCollider)
                {
                    row = r;
                    col = c;
                    return true;
                }
            }
        }

        row = -1;
        col = -1;
        return false;
    }
    
    /// <summary>
    /// 주어진 월드 좌표가 속한 PolygonCollider2D를 찾아 반환합니다.
    /// </summary>
    public bool TryGetColliderAtPosition(Vector3 worldPos, out PolygonCollider2D outCollider)
    {
        for (int r = 0; r < coliderMat.Length; r++)
        {
            var rowArr = coliderMat[r].arr_row;
            if (rowArr == null) continue;

            for (int c = 0; c < rowArr.Length; c++)
            {
                var poly = rowArr[c];
                if (poly == null) continue;

                if (poly.OverlapPoint(worldPos))
                {
                    outCollider = poly;
                    return true;
                }
            }
        }

        outCollider = null;
        return false;
    }

    /// <summary>
    /// 주어진 Transform의 위치가 속한 PolygonCollider2D를 찾아 반환합니다.
    /// </summary>
    public bool TryGetColliderForTransform(Transform t, out PolygonCollider2D outCollider)
    {
       
        return TryGetColliderAtPosition((Vector3)t.position, out outCollider);
    }
    
    public List<GameObject> GetPatternGameObjects(DetectionPatternSO patternSo, AllyTile occupiedTile)
    {
        var result = new List<GameObject>();
        if (patternSo == null || occupiedTile == null || occupiedTile._hitCollider == null)
            return result;

        // 기준이 되는 타일의 콜라이더
        var baseCollider = occupiedTile._hitCollider;

        // 기준 타일의 (row, col) 얻기
        if (!TryGetGridIndex(baseCollider, out int baseRow, out int baseCol))
            return result;

        // 패턴 오프셋 순회
        foreach (var ofs in patternSo.cellOffsets)
        {
            // dir에 따라 Y축 반전 처리 (위/아래 구분)
            var applied = occupiedTile.dir
                ? new Vector2Int(ofs.x, -ofs.y)
                : new Vector2Int(ofs.x, ofs.y);

            int row = baseRow + applied.y;
            int col = baseCol + applied.x;

            // 범위 검사
            if (row < 0 || row >= coliderMat.Length) continue;
            if (col < 0 || col >= coliderMat[row].arr_row.Length) continue;

            // 해당 위치의 콜라이더 가져오기
            var poly = coliderMat[row].arr_row[col];
            if (poly == null) continue;

            var go = poly.gameObject;
            if (!result.Contains(go))
                result.Add(go);
        }

        return result;
    }
}
