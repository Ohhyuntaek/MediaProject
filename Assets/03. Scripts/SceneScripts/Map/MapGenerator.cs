using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [Header("맵 설정")]
    public int width = 7;
    public int height = 8;

    [Header("UI 설정")]
    public float xSpacing = 160f;
    public float ySpacing = 200f;
    public float xJitter = 30f;
    public float yJitter = 15f;
    [Range(0f, 1f)]
    public float nodeSpawnChance = 0.6f;

    [Header("노드 프리팹")]
    public GameObject nodeButtonPrefab;
    public Transform gridOrigin;
    public GameObject linePrefab;
    public Transform lineContainer;

    [Header("스테이지 데이터 풀")]
    public List<StageData> normalStages;
    public List<StageData> shopStages;
    public List<StageData> bossStages;

    [Header("디버그용 그리드")]
    public Transform gridLineContainer;
    public GameObject gridLinePrefab;
    public Color gridColor = Color.gray;
    public float lineWidth = 2f;

    [SerializeField]
    private GameObject markerPrefab;

    private GameObject markerInstance;
    private StageNodeVer2[,] grid;
    private Dictionary<StageNodeVer2, GameObject> nodeButtons = new();
    private StageNodeVer2 currentNode;

    void Start()
    {
        GenerateDebugGridLines();

        if (RuntimeDataManager.Instance.mapGenerated)
        {
            // 기존 맵 로드
            LoadMapFromRuntime();

            if (RuntimeDataManager.Instance.nextNode != null)
            {
                currentNode = RuntimeDataManager.Instance.nextNode;
                RuntimeDataManager.Instance.currentNode = currentNode;
                RuntimeDataManager.Instance.nextNode = null;
            }
        }
        else
        {
            // 새 맵 생성 (경로 구성 포함)
            GenerateValidMap();

            // 여기에서 연결 수 적은 노드 제거 (중요 경로 제외)
            RemoveWeaklyConnectedNodes();

            // 맵 정보 저장
            SaveMapToRuntime();

            // 시작 노드는 자동 클리어
            currentNode = grid[0, 0];
            currentNode.IsCleared = true;
        }

        // 반드시 노드 정리 후 UI 버튼 생성
        GenerateNodeButtons();

        // 현재 위치에서 도달 가능한 노드 활성화
        HighlightAvailableNodes(currentNode);

        // 마커 표시
        UpdateMarkerPosition(currentNode);

        // 노드 UI 초기화
        foreach (var btn in nodeButtons.Values)
            btn.GetComponent<NodeButton>().Refresh();
    }


    #region Map 생성 및 유효성 검증

    private void GenerateDebugGridLines()
    {
        float totalWidth = (width - 1) * xSpacing;
        float totalHeight = (height - 1) * ySpacing;

        // 세로 라인
        for (int x = 0; x < width; x++)
        {
            GameObject line = Instantiate(gridLinePrefab, gridLineContainer);
            var rt = line.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(lineWidth, totalHeight + ySpacing);
            rt.anchoredPosition = new Vector2(x * xSpacing, totalHeight / 2f);
            rt.localRotation = Quaternion.identity;
            line.GetComponent<Image>().color = gridColor;
        }

        // 가로 라인
        for (int y = 0; y < height; y++)
        {
            GameObject line = Instantiate(gridLinePrefab, gridLineContainer);
            var rt = line.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(totalWidth + xSpacing, lineWidth);
            rt.anchoredPosition = new Vector2(totalWidth / 2f, y * ySpacing);
            rt.localRotation = Quaternion.identity;
            line.GetComponent<Image>().color = gridColor;
        }
    }
    
    // 유효한 맵 생성 및 연결
    void GenerateValidMap()
    {
        for (int attempt = 0; attempt < 30; attempt++)
        {
            StageNodeVer2[,] tempGrid;
            List<StageNodeVer2> mainPath = GenerateLogicalGrid(out tempGrid);
            ForcePlaceBossNode(ref tempGrid);
            ForceConnectPath(mainPath);

            grid = tempGrid;
            AssignStageTypes();
            ConnectNodes();

            var reachable = CollectConnectedNodes(grid[0, 0]);
            var bossNode = grid[width - 1, height - 1];

            // 보스가 도달 가능하면 종료
            if (reachable.Contains(bossNode))
                break;

            // 도달 불가능하면 보스까지 강제로 연결
            ForceConnectToBoss(bossNode, reachable);
            reachable = CollectConnectedNodes(grid[0, 0]);

            if (reachable.Contains(bossNode))
                break;
        }

        CleanDisconnectedNodes();
        ConnectNodes();
    }

    // 보스 노드 강제 배치
    void ForcePlaceBossNode(ref StageNodeVer2[,] tempGrid)
    {
        if (tempGrid[width - 1, height - 1] == null)
            tempGrid[width - 1, height - 1] = new StageNodeVer2(width - 1, height - 1);
    }

    // mainPath 연결
    void ForceConnectPath(List<StageNodeVer2> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var from = path[i];
            var to = path[i + 1];
            from.ConnectedNodes.Add(to);
            to.IncomingNodes.Add(from);
        }
    }

    // 보스까지 연결되지 않은 경우, 가장 가까운 노드와 연결
    void ForceConnectToBoss(StageNodeVer2 bossNode, HashSet<StageNodeVer2> reachable)
    {
        StageNodeVer2 nearest = null;
        float minDist = float.MaxValue;

        foreach (var node in reachable)
        {
            float dist = Vector2Int.Distance(node.GridPosition, bossNode.GridPosition);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }

        if (nearest != null)
        {
            nearest.ConnectedNodes.Add(bossNode);
            bossNode.IncomingNodes.Add(nearest);
        }
    }

    // 도달 불가능한 노드 제거
    void CleanDisconnectedNodes()
    {
        var fromStart = CollectConnectedNodes(grid[0, 0]);
        var toBoss = CollectNodesReachableFrom(grid[width - 1, height - 1]);

        var validNodes = new HashSet<StageNodeVer2>(fromStart);
        validNodes.IntersectWith(toBoss);
        validNodes.Add(grid[0, 0]);
        validNodes.Add(grid[width - 1, height - 1]);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node != null && !validNodes.Contains(node))
                    grid[x, y] = null;
            }
        }

        PruneUnreachableNodes();
    }
    
    /// <summary>
    /// 연결 수가 너무 적은 노드 제거 (단, 시작/보스/중요 경로 노드는 유지)
    /// </summary>
    private void RemoveWeaklyConnectedNodes()
    {
        // 중요한 경로 노드 수집
        var start = grid[0, 0];
        var boss = grid[width - 1, height - 1];
        var forward = CollectConnectedNodes(start);
        var backward = CollectNodesReachableFrom(boss);

        var essential = new HashSet<StageNodeVer2>(forward);
        essential.IntersectWith(backward);
        essential.Add(start);
        essential.Add(boss);

        List<Vector2Int> toRemove = new();

        // 제거 대상 수집
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var node = grid[x, y];
            if (node == null) continue;
            if (essential.Contains(node)) continue;

            if (node.TotalConnections <= 1)
                toRemove.Add(new Vector2Int(x, y));
        }

        // 제거 실행
        foreach (var pos in toRemove)
        {
            var node = grid[pos.x, pos.y];
            if (node == null) continue;

            foreach (var prev in node.IncomingNodes)
                prev.ConnectedNodes.Remove(node);

            foreach (var next in node.ConnectedNodes)
                next.IncomingNodes.Remove(node);

            grid[pos.x, pos.y] = null;
        }

        // 연결 재정리
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var node = grid[x, y];
            if (node == null) continue;

            node.ConnectedNodes.RemoveAll(n => !IsValid(n));
            node.IncomingNodes.RemoveAll(n => !IsValid(n));
        }

        bool IsValid(StageNodeVer2 n)
        {
            var p = n.GridPosition;
            return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height && grid[p.x, p.y] != null;
        }
    }

    #endregion

    #region Stage 배치 및 연결

    List<StageNodeVer2> GenerateLogicalGrid(out StageNodeVer2[,] generatedGrid)
    {
        generatedGrid = new StageNodeVer2[width, height];
        List<StageNodeVer2> mainPath = new();

        int x = 0, y = 0;
        var node = new StageNodeVer2(x, y);
        generatedGrid[x, y] = node;
        mainPath.Add(node);

        while (x < width - 1 || y < height - 1)
        {
            bool canRight = x < width - 1;
            bool canUp = y < height - 1;
            bool moveRight = canRight && (!canUp || Random.value < 0.5f);

            if (moveRight) x++;
            else y++;

            if (generatedGrid[x, y] == null)
            {
                node = new StageNodeVer2(x, y);
                generatedGrid[x, y] = node;
            }
            else
            {
                node = generatedGrid[x, y];
            }

            mainPath.Add(node);
        }

        // 랜덤 노드 추가
        for (int j = 0; j < height; j++)
            for (int i = 0; i < width; i++)
                if (generatedGrid[i, j] == null && Random.value < nodeSpawnChance)
                    generatedGrid[i, j] = new StageNodeVer2(i, j);

        return mainPath;
    }

    void AssignStageTypes()
    {
        int shopCount = 0;
        int maxShops = 4;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var node = grid[x, y];
            if (node == null) continue;

            if (x == 0 && y == 0)
            {
                node.StageData = GetRandomStage(StageType.Normal);
                continue;
            }

            if (x == width - 1 && y == height - 1)
            {
                node.StageData = GetRandomStage(StageType.Boss);
                continue;
            }

            StageType type;
            if (shopCount < maxShops && y >= height / 2 && Random.value < 0.25f)
            {
                type = StageType.Shop;
                shopCount++;
            }
            else
            {
                type = StageType.Normal;
            }

            node.StageData = GetRandomStage(type);
        }

        // 보정
        if (shopCount == 0)
            ForcePlaceAShop();

        foreach (var node in grid)
        {
            if (node != null && node.StageData == null)
                node.StageData = GetRandomStage(StageType.Normal);
        }
    }

    StageData GetRandomStage(StageType type)
    {
        return type switch
        {
            StageType.Normal => normalStages[Random.Range(0, normalStages.Count)],
            StageType.Shop => shopStages[Random.Range(0, shopStages.Count)],
            StageType.Boss => bossStages[Random.Range(0, bossStages.Count)],
            _ => null
        };
    }

    void ForcePlaceAShop()
    {
        var candidates = grid.Cast<StageNodeVer2>()
            .Where(n => n != null && n.StageData.StageType == StageType.Normal)
            .ToList();

        if (candidates.Count > 0)
            candidates[Random.Range(0, candidates.Count)].StageData = GetRandomStage(StageType.Shop);
    }

    #endregion
    #region 노드 연결 및 UI

    // 노드 간 연결 생성
    void ConnectNodes()
    {
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var current = grid[x, y];
                if (current == null) continue;

                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    int ny = y + 1;
                    if (nx < 0 || nx >= width) continue;

                    var next = grid[nx, ny];
                    if (next != null)
                    {
                        current.ConnectedNodes.Add(next);
                        next.IncomingNodes.Add(current);
                    }
                }
            }
        }
    }

    // 연결되지 않은 고립 노드 제거
    void PruneUnreachableNodes()
    {
        List<Vector2Int> toRemove = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                bool isStart = (x == 0 && y == 0);
                bool isEnd = (x == width - 1 && y == height - 1);

                if (isStart || isEnd) continue;

                int inCount = node.IncomingNodes.Count(n => IsValid(n));
                int outCount = node.ConnectedNodes.Count(n => IsValid(n));

                if (inCount == 0 || outCount == 0 || (inCount <= 1 && outCount <= 1))
                    toRemove.Add(new Vector2Int(x, y));
            }
        }

        foreach (var pos in toRemove)
        {
            var node = grid[pos.x, pos.y];
            if (node == null) continue;

            foreach (var prev in node.IncomingNodes)
                prev.ConnectedNodes.Remove(node);
            foreach (var next in node.ConnectedNodes)
                next.IncomingNodes.Remove(node);

            grid[pos.x, pos.y] = null;
        }

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var node = grid[x, y];
            if (node == null) continue;
            node.ConnectedNodes.RemoveAll(n => !IsValid(n));
            node.IncomingNodes.RemoveAll(n => !IsValid(n));
        }

        bool IsValid(StageNodeVer2 n)
        {
            var p = n.GridPosition;
            return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height && grid[p.x, p.y] != null;
        }
    }

    // UI 노드 버튼 생성
    void GenerateNodeButtons()
    {
        nodeButtons.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                GameObject btn = Instantiate(nodeButtonPrefab, gridOrigin);
                var rt = btn.GetComponent<RectTransform>();

                Vector2 offset;
                if (node.CachedPosition.HasValue)
                    offset = node.CachedPosition.Value;
                else
                {
                    float xOffset = x * xSpacing + Random.Range(-xJitter, xJitter);
                    float yOffset = y * ySpacing + Random.Range(-yJitter, yJitter);
                    offset = new Vector2(xOffset, yOffset);
                    node.CachedPosition = offset;
                }

                rt.anchoredPosition = offset;
                btn.GetComponent<NodeButton>().Init(node);
                nodeButtons[node] = btn;
            }
        }

        DrawConnections();
    }

    // 노드 간 선 그리기
    void DrawConnections()
    {
        foreach (var pair in nodeButtons)
        {
            var fromNode = pair.Key;
            var fromRT = pair.Value.GetComponent<RectTransform>();

            foreach (var toNode in fromNode.ConnectedNodes)
            {
                if (!nodeButtons.TryGetValue(toNode, out var toObj)) continue;

                var toRT = toObj.GetComponent<RectTransform>();
                Vector2 start = fromRT.anchoredPosition;
                Vector2 end = toRT.anchoredPosition;
                Vector2 dir = end - start;

                GameObject line = Instantiate(linePrefab, lineContainer);
                RectTransform lineRT = line.GetComponent<RectTransform>();
                lineRT.sizeDelta = new Vector2(dir.magnitude, 4f);
                lineRT.anchoredPosition = start + dir / 2f;
                lineRT.rotation = Quaternion.FromToRotation(Vector3.right, dir);
            }
        }
    }

    // 현재 노드 기준으로 이동 가능한 노드 활성화
    void HighlightAvailableNodes(StageNodeVer2 current)
    {
        foreach (var pair in nodeButtons)
        {
            var node = pair.Key;
            var button = pair.Value;

            bool isReachable = current.ConnectedNodes.Contains(node)
                               || current.IncomingNodes.Contains(node)
                               || node.IncomingNodes.Contains(current);

            button.GetComponent<Button>().interactable = isReachable;
        }
    }

    // 마커 위치 업데이트
    void UpdateMarkerPosition(StageNodeVer2 node)
    {
        if (!nodeButtons.TryGetValue(node, out var button)) return;

        if (markerInstance == null)
            markerInstance = Instantiate(markerPrefab, button.transform.parent);

        markerInstance.SetActive(true);
        markerInstance.transform.SetAsLastSibling();
        markerInstance.transform.position = button.transform.position;
    }

    #endregion

    #region 경로 수집

    // 시작 노드부터 연결된 모든 노드 수집
    HashSet<StageNodeVer2> CollectConnectedNodes(StageNodeVer2 start)
    {
        var visited = new HashSet<StageNodeVer2>();
        var stack = new Stack<StageNodeVer2>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node == null || visited.Contains(node)) continue;

            visited.Add(node);
            foreach (var next in node.ConnectedNodes) stack.Push(next);
            foreach (var prev in node.IncomingNodes) stack.Push(prev);
        }

        return visited;
    }

    // 특정 노드로 도달 가능한 모든 노드 수집
    HashSet<StageNodeVer2> CollectNodesReachableFrom(StageNodeVer2 end)
    {
        var visited = new HashSet<StageNodeVer2>();
        var stack = new Stack<StageNodeVer2>();
        stack.Push(end);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node == null || visited.Contains(node)) continue;

            visited.Add(node);
            foreach (var prev in node.IncomingNodes) stack.Push(prev);
        }

        return visited;
    }

    #endregion

    #region 저장/로드 및 씬 이동

    void SaveMapToRuntime()
    {
        RuntimeDataManager.Instance.mapGrid = grid;
        RuntimeDataManager.Instance.currentNode = currentNode;
        RuntimeDataManager.Instance.mapGenerated = true;
    }

    void LoadMapFromRuntime()
    {
        grid = RuntimeDataManager.Instance.mapGrid;
        currentNode = RuntimeDataManager.Instance.currentNode;
    }

    public void OnNodeSelected(StageNodeVer2 selectedNode)
    {
        bool isReachable = currentNode != null &&
                           (currentNode.ConnectedNodes.Contains(selectedNode)
                            || currentNode.IncomingNodes.Contains(selectedNode)
                            || selectedNode.IncomingNodes.Contains(currentNode));

        if (!isReachable) return;

        currentNode = selectedNode;
        RuntimeDataManager.Instance.currentNode = currentNode;
        HighlightAvailableNodes(currentNode);
        UpdateMarkerPosition(currentNode);

        if (selectedNode.IsCleared)
        {
            Debug.Log("이미 클리어한 노드에 도착함. 씬 전환 없음.");
            return;
        }

        RuntimeDataManager.Instance.nextNode = selectedNode;
        RuntimeDataManager.Instance.currentStageData = selectedNode.StageData;

        switch (selectedNode.StageData.StageType)
        {
            case StageType.Normal:
            case StageType.Boss:
                LoadingSceneManager.LoadScene("InStage");
                break;
            case StageType.Shop:
                LoadingSceneManager.LoadScene("ShopScene");
                break;
        }
    }

    #endregion
}

