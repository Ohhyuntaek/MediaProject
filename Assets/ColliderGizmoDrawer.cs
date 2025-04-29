using UnityEngine;
using System.Collections.Generic;

// 에디트 모드에서도 실행하려면 이 어트리뷰트를 추가하세요
[ExecuteAlways]
public class PolygonCollider2DListDrawer : MonoBehaviour
{
    [Tooltip("씬 뷰에 그려줄 PolygonCollider2D 들을 여기에 추가하세요")]
    public List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();

    [Tooltip("Gizmo 색상")]
    public Color color = Color.cyan;

    private void OnDrawGizmos()
    {
        if (colliders == null || colliders.Count == 0)
            return;

        Gizmos.color = color;
        foreach (var poly in colliders)
        {
            if (poly == null) continue;

            // 각 path(각 다이아몬드)에 대해
            for (int p = 0; p < poly.pathCount; p++)
            {
                Vector2[] pts = poly.GetPath(p);
                for (int i = 0; i < pts.Length; i++)
                {
                    // 로컬 좌표 → 월드 좌표
                    Vector3 a = poly.transform.TransformPoint(pts[i]);
                    Vector3 b = poly.transform.TransformPoint(pts[(i + 1) % pts.Length]);
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}