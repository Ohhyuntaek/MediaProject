using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵의 각 스테이지 노드 정보
/// </summary>
public class StageNodeVer2
{
    // 그리드 좌표
    public Vector2Int GridPosition;

    // 이 노드의 스테이지 데이터 (타입 포함)
    public StageData StageData;

    // 다음 층으로 연결된 노드 목록
    public List<StageNodeVer2> ConnectedNodes;

    // 이전 층에서 이 노드로 연결된 노드 목록
    public List<StageNodeVer2> IncomingNodes;
    
    public bool IsCleared = false;

    public Vector2? CachedPosition = null;
    
    // 생성자
    public StageNodeVer2(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
        ConnectedNodes = new List<StageNodeVer2>();
        IncomingNodes = new List<StageNodeVer2>();
    }
}