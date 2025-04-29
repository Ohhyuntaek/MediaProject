using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RaycastTileHighlighter2D : MonoBehaviour
{
    [Header("타일맵 설정")] public Tilemap _tilemap; // TargetPass/Left 또는 …/Right 타일맵
    public float rayDistance = 10f; // 레이캐스트 최대 거리
    public LayerMask tileLayerMask; // 타일맵 콜라이더 레이어

    [Header("디버그 색상")] public Color hitTileColor = Color.red;
    public Color rangeTileColor = Color.yellow;

    [HideInInspector] public Vector3 hitCellPos;


    private void Awake()
    {
        // 1) 이미 인스펙터에 할당되어 있으면 그대로 쓰고
        if (_tilemap != null) return;

        // 2) TargetPass 아래 Left/Right 이름으로 찾아서 자동 할당
        var targetPass = GameObject.Find("TargetPass");
        if (targetPass == null)
        {
            Debug.LogError("Scene에 TargetPass 오브젝트가 없습니다.");
            return;
        }

        // 자신의 이름을 비교해서 Left/Right 구분
        var myName = gameObject.name.ToLower();
        Transform child = null;
        if (myName.Contains("left"))
            child = targetPass.transform.Find("Left");
        else if (myName.Contains("right"))
            child = targetPass.transform.Find("Right");

        if (child != null)
        {
            _tilemap = child.GetComponent<Tilemap>();
            if (_tilemap == null)
                Debug.LogError($"{child.name}에 Tilemap 컴포넌트가 없습니다.");
        }
        else
        {
            Debug.LogError($"TargetPass 하위에 '{gameObject.name}' 오브젝트를 찾지 못했습니다.");
        }
    }

   
  

    private void OnDrawGizmos()
    {
        DrawRayGizmo(Color.cyan);
    }

    // 씬 뷰에서 선택한 객체에만 Gizmo를 그리려면 이걸 사용하세요.
//  private void OnDrawGizmosSelected()
//  {
//      DrawRayGizmo(Color.yellow);
//  }

    // 실제 레이와 동일하게 그리기 위한 공통 함수
    private void DrawRayGizmo(Color gizmoColor)
    {
        Gizmos.color = gizmoColor;

        Vector3 origin    = transform.position;
        Vector3 direction = transform.right;

        // 레이를 선으로 시각화
        Gizmos.DrawLine(origin, origin + direction * rayDistance);

        // 끝점에 작은 구 표시 (선택 시 눈에 더 잘 띕니다)
        Gizmos.DrawSphere(origin + direction * rayDistance, 0.05f);
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
}
