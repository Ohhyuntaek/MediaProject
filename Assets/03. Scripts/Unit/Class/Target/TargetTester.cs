using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RaycastTileHighlighter2D : MonoBehaviour
{
    [Header("타일맵 설정")]
    public Tilemap _tilemap;           // 레이를 맞출 타일맵 (TilemapCollider2D가 적용된 타일맵)
    public float rayDistance = 10f;   // 레이캐스트 최대 거리
    public LayerMask tileLayerMask;   // 타일맵 콜라이더가 속한 레이어

    [Header("공격 사정거리 설정")]
    public int attackRange = 3;       // 공격 사정거리 (칸 단위)
    
    [Header("디버그 색상")]
    public Color hitTileColor = Color.red;       // 레이로 맞춘 타일 색상
    public Color rangeTileColor = Color.yellow;    // 공격 사정거리 칸 색상
    
    public Vector3Int? hitCellPos = null;       // 레이로 맞춘 타일의 셀 좌표
    public Vector3Int[] rangeCells = null;      // 맞춘 타일에서 오른쪽으로 attackRange 칸의 셀 좌표들

    
    public void Awake()
    {
        GameObject targetPass = GameObject.Find("TargetPass");
        if (targetPass == null)
        {
            Debug.LogError("TargetPass 오브젝트를 씬에서 찾을 수 없습니다.");
            return;
        }

        // 현재 게임 오브젝트의 이름을 확인 (대소문자 구분 없이)
        string objName = gameObject.name;

        if (objName == "Left")
        {
            // TargetPass 하위에 "Left"라는 이름의 자식을 찾습니다.
            Transform leftTransform = targetPass.transform.Find("Left");
            if (leftTransform != null)
            {
                _tilemap = leftTransform.GetComponent<Tilemap>();
                if (_tilemap == null)
                {
                    Debug.LogError("TargetPass/Left 오브젝트에 Tilemap 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("TargetPass 하위에 'Left' 오브젝트를 찾을 수 없습니다.");
            }
        }
        else if (objName == "Right")
        {
            // TargetPass 하위에 "Right"라는 이름의 자식을 찾습니다.
            Transform rightTransform = targetPass.transform.Find("Right");
            if (rightTransform != null)
            {
                _tilemap = rightTransform.GetComponent<Tilemap>();
                if (_tilemap == null)
                {
                    Debug.LogError("TargetPass/Right 오브젝트에 Tilemap 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("TargetPass 하위에 'Right' 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("게임 오브젝트 이름이 'LEFT' 또는 'RIGHT'가 아닙니다.");
        }
    }

    public bool GetHitTileMap()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, rayDistance, tileLayerMask);
        if (hitInfo.collider.CompareTag("TargetTileRight"))
        {
            return true;
        }
        else
        {

            return false;
        }
    }

    public void DetectTiles(int attackRange)
    {
        // 캐릭터의 위치에서 transform.right 방향으로 레이캐스트 (2D)
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, rayDistance, tileLayerMask);

        if (hitInfo.collider != null)
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3Int cellPos = _tilemap.WorldToCell(hitPoint);
            if (_tilemap.HasTile(cellPos))
            {
                hitCellPos = cellPos;
                rangeCells = new Vector3Int[attackRange];
                for (int i = 0; i < attackRange; i++)
                {
                    rangeCells[i] = new Vector3Int(cellPos.x + (i + 1), cellPos.y, cellPos.z);
                }
            }
            else
            {
                hitCellPos = null;
                rangeCells = null;
            }
        }
        else
        {
            hitCellPos = null;
            rangeCells = null;
        }
    }

    public string GetTargetTileTag()
    {
        // 캐릭터의 위치에서 transform.right 방향으로 레이캐스트 (2D)
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, rayDistance, tileLayerMask);

        if (hitInfo.collider != null)
        {
            return hitInfo.collider.tag;
        }
        else
        {
            return null;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        // 캐릭터에서 발사한 레이를 파란색 선으로 그립니다.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * rayDistance);

        if (_tilemap == null)
            return;

        // 레이로 맞춘 타일(hitCellPos) 그리기
        if (hitCellPos.HasValue)
        {
            // hitTile을 빨간색 다이아몬드로 표시
            DrawIsometricDiamond(hitCellPos.Value, hitTileColor);

            // hitTile 위치에도 초록색 구체를 그려 OverlapPointAll 검사 지점을 시각화
            Vector3 hitCenter = _tilemap.GetCellCenterWorld(hitCellPos.Value);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitCenter, 0.1f);

            // 사정거리 칸(rangeCells)도 그리기
            if (rangeCells != null)
            {
                foreach (Vector3Int cell in rangeCells)
                {
                    if (_tilemap.HasTile(cell))
                    {
                        DrawIsometricDiamond(cell, rangeTileColor);

                        // 각 셀 중심에 작은 구체를 그려서 OverlapPointAll 검사 지점을 시각화
                        Vector3 cellCenter = _tilemap.GetCellCenterWorld(cell);
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(cellCenter, 0.1f);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 타일맵의 셀 좌표에 해당하는 타일의 중앙을 기준으로 아이소메트릭 마름모(다이아몬드) 형태를 그립니다.
    /// </summary>
    /// <param name="cellPos">타일의 셀 좌표</param>
    /// <param name="color">그릴 색상</param>
    private void DrawIsometricDiamond(Vector3Int cellPos, Color color)
    {
        Vector3 center = _tilemap.GetCellCenterWorld(cellPos);
        float halfX = _tilemap.cellSize.x / 2f;
        float halfY = _tilemap.cellSize.y / 2f;
        Vector3 top = center + new Vector3(0, halfY, 0);
        Vector3 right = center + new Vector3(halfX, 0, 0);
        Vector3 bottom = center + new Vector3(0, -halfY, 0);
        Vector3 left = center + new Vector3(-halfX, 0, 0);
        Gizmos.color = color;
        Gizmos.DrawLine(top, right);
        Gizmos.DrawLine(right, bottom);
        Gizmos.DrawLine(bottom, left);
        Gizmos.DrawLine(left, top);
    }
}
