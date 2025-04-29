using System;
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
}