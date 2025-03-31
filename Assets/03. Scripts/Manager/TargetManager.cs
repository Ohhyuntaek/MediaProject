using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [Header("설정")]
    public float tileDistance = 1.73f; // isometric 타일 간 거리
    public int detectionRange = 3;
    public LayerMask targetLayer; // 적 유닛 레이어

    [Header("디버그")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;

    /// <summary>
    /// 지정 방향으로 Raycast를 발사하여 Enemy 감지
    /// </summary>
    public List<Enemy> DetectEnemiesInDirection(Vector3 direction)
    {
        List<Enemy> targets = new List<Enemy>();

        for (int i = 1; i <= detectionRange; i++)
        {
            Vector3 start = transform.position + direction * tileDistance * i;
            Ray ray = new Ray(start + Vector3.up * 1f, Vector3.down); // 위에서 아래로

            if (Physics.Raycast(ray, out RaycastHit hit, 2f, targetLayer))
            {
                if (hit.collider.TryGetComponent(out Enemy enemy))
                {
                    targets.Add(enemy);
                }
            }
        }

        return targets;
    }

    /// <summary>
    /// 현재 forward 방향 기준으로 적 감지 (예시용)
    /// </summary>
    public List<Enemy> DetectForwardEnemies()
    {
        return DetectEnemiesInDirection(transform.forward);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;

        for (int i = 1; i <= detectionRange; i++)
        {
            Vector3 pos = transform.position + transform.forward * tileDistance * i;
            Gizmos.DrawWireCube(pos + Vector3.up * 0.5f, Vector3.one * 0.5f);
        }
    }
}