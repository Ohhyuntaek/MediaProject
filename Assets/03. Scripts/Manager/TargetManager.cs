using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackAreaGizmoDrawer : MonoBehaviour
{
    [Header("타일맵 설정")]
    public Tilemap enemyPathTilemap; // 적군 경로 타일맵 (TilemapCollider2D가 적용된 타일맵)
    
    [Header("공격 범위 설정")]
    public int attackRange = 3; // 공격 사정거리 (칸 단위, 최소 1 이상)

    private void OnDrawGizmosSelected()
    {
        if (enemyPathTilemap == null)
            return;

        // 유닛의 현재 위치를 기준으로 타일맵 셀 좌표로 변환
        Vector3Int unitCell = enemyPathTilemap.WorldToCell(transform.position);

        // "바로 앞 한 칸"은 unitCell에서 y+1, 그리고 그 칸에서 오른쪽으로 attackRange만큼의 셀들을 그립니다.
        for (int offsetX = 0; offsetX < attackRange; offsetX++)
        {
            // 셀 좌표: x는 unitCell.x + offsetX, y는 unitCell.y + 1 (바로 앞)
            Vector3Int targetCell = new Vector3Int(unitCell.x + offsetX, unitCell.y + 1, unitCell.z);
            
            // 셀의 중앙 위치 (아이소메트릭 타일맵에서는 GetCellCenterWorld()가 유용)
            Vector3 cellCenter = enemyPathTilemap.GetCellCenterWorld(targetCell);
            
            // 아이소메트릭 타일 모양(마름모)를 그리기 위한 각 꼭짓점 계산
            Vector3 top = cellCenter + new Vector3(0, enemyPathTilemap.cellSize.y / 2f, 0);
            Vector3 right = cellCenter + new Vector3(enemyPathTilemap.cellSize.x / 2f, 0, 0);
            Vector3 bottom = cellCenter + new Vector3(0, -enemyPathTilemap.cellSize.y / 2f, 0);
            Vector3 left = cellCenter + new Vector3(-enemyPathTilemap.cellSize.x / 2f, 0, 0);

            // Gizmo 색상 설정
            Gizmos.color = Color.red;
            // 네 꼭짓점을 연결하여 마름모(다이아몬드) 형태로 그립니다.
            Gizmos.DrawLine(top, right);
            Gizmos.DrawLine(right, bottom);
            Gizmos.DrawLine(bottom, left);
            Gizmos.DrawLine(left, top);
        }
    }
}