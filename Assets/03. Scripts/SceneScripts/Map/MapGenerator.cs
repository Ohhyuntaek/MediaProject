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
    public Transform gridLineContainer;     // 격자 부모
    public GameObject gridLinePrefab;       // 얇은 Image 또는 Line 프리팹
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
            StageNodeVer2 bossNode = null;
            bool success = false;

            for (int attempt = 0; attempt < 30 && !success; attempt++)
            {
                StageNodeVer2[,] tempGrid;
                List<StageNodeVer2> mainPath = GenerateLogicalGrid(out tempGrid);

                // 💡 보스 노드 강제 생성
                if (tempGrid[width - 1, height - 1] == null)
                    tempGrid[width - 1, height - 1] = new StageNodeVer2(width - 1, height - 1);

                bossNode = tempGrid[width - 1, height - 1];

                // mainPath 강제 연결
                for (int i = 0; i < mainPath.Count - 1; i++)
                {
                    var from = mainPath[i];
                    var to = mainPath[i + 1];
                    from.ConnectedNodes.Add(to);
                    to.IncomingNodes.Add(from);
                }

                grid = tempGrid;
                AssignStageTypes();
                ConnectNodes();

                var reachable = CollectConnectedNodes(grid[0, 0]);

                if (!reachable.Contains(bossNode))
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

                    reachable = CollectConnectedNodes(grid[0, 0]);
                }

                if (reachable.Contains(bossNode))
                    success = true;
            }

            if (!success)
            {
                Debug.LogError("보스 노드까지 연결된 경로 생성 실패");
                return;
            }

            var fromStart = CollectConnectedNodes(grid[0, 0]);
            var toBoss = CollectNodesReachableFrom(bossNode);
            var validNodes = new HashSet<StageNodeVer2>(fromStart);
            validNodes.IntersectWith(toBoss);
            validNodes.Add(grid[0, 0]);
            validNodes.Add(bossNode);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var node = grid[x, y];
                    if (node == null) continue;

                    bool isStart = (x == 0 && y == 0);
                    bool isBoss = (x == width - 1 && y == height - 1);

                    if (!validNodes.Contains(node) && !isStart && !isBoss)
                        grid[x, y] = null;
                }
            }

            ConnectNodes();
            PruneUnreachableNodes();
            ConnectNodes();

            currentNode = grid[0, 0];
            currentNode.IsCleared = true;

            SaveMapToRuntime();
        }

        GenerateNodeButtons();
        HighlightAvailableNodes(currentNode);
        UpdateMarkerPosition(currentNode);

        foreach (var btn in nodeButtons.Values)
            btn.GetComponent<NodeButton>().Refresh();
    }
    
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

    private void SaveMapToRuntime()
    {
        RuntimeDataManager.Instance.mapGrid = grid;
        RuntimeDataManager.Instance.currentNode = currentNode;
        RuntimeDataManager.Instance.mapGenerated = true;
    }

    private void LoadMapFromRuntime()
    {
        grid = RuntimeDataManager.Instance.mapGrid;
        currentNode = RuntimeDataManager.Instance.currentNode;
    }

    private List<StageNodeVer2> GenerateLogicalGrid(out StageNodeVer2[,] generatedGrid)
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

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (generatedGrid[i, j] == null && Random.value < nodeSpawnChance)
                {
                    generatedGrid[i, j] = new StageNodeVer2(i, j);
                }
            }
        }

        return mainPath;
    }


    private void AssignStageTypes()
    {
        int shopCount = 1;
        int maxShops = 4;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                // 시작 노드는 반드시 Normal
                if (x == 0 && y == 0)
                {
                    node.StageData = GetRandomStage(StageType.Normal);
                    continue;
                }

                // 보스 노드는 반드시 Boss
                if (x == width - 1 && y == height - 1)
                {
                    node.StageData = GetRandomStage(StageType.Boss);
                    continue;
                }

                // 나머지 노드 랜덤 지정
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
        }

        // ✅ 예외 방어: StageData가 누락된 노드가 있는지 최종 검수
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node != null && node.StageData == null)
                {
                    Debug.LogWarning($"[경고] ({x},{y}) 노드에 StageData가 비어 있어 Normal로 지정합니다.");
                    node.StageData = GetRandomStage(StageType.Normal);
                }
            }
        }

        // ✅ 최소 1개는 Shop이 존재하도록 강제
        if (shopCount == 0)
            ForcePlaceAShop();
    }


    private StageData GetRandomStage(StageType type)
    {
        switch (type)
        {
            case StageType.Normal: return normalStages[Random.Range(0, normalStages.Count)];
            case StageType.Shop: return shopStages[Random.Range(0, shopStages.Count)];
            case StageType.Boss: return bossStages[Random.Range(0, bossStages.Count)];
            default: return null;
        }
    }

    private void ForcePlaceAShop()
    {
        List<StageNodeVer2> candidates = new();
        foreach (var node in grid)
        {
            if (node != null && node.StageData.StageType == StageType.Normal)
                candidates.Add(node);
        }

        if (candidates.Count > 0)
        {
            var chosen = candidates[Random.Range(0, candidates.Count)];
            chosen.StageData = GetRandomStage(StageType.Shop);
        }
    }

    private void ConnectNodes()
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

    private void PruneUnreachableNodes()
    {
        List<Vector2Int> toRemove = new();

        // 1. 제거 대상 수집
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                bool isStart = (x == 0 && y == 0);
                bool isEnd = (x == width - 1 && y == height - 1);

                if (isStart || isEnd)
                    continue;

                int inCount = node.IncomingNodes.Count(n => IsValid(n));
                int outCount = node.ConnectedNodes.Count(n => IsValid(n));

                // ✅ 제거 조건: 부모 없음, 자식 없음, 또는 연결이 양쪽 1개 이하 (통과만 하는 노드)
                if (inCount == 0 || outCount == 0 || (inCount <= 1 && outCount <= 1))
                {
                    toRemove.Add(new Vector2Int(x, y));
                }
            }
        }

        // 2. 연결 정리 + 제거
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

        // 3. 남은 노드의 연결 다시 정리
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                node.ConnectedNodes.RemoveAll(n => !IsValid(n));
                node.IncomingNodes.RemoveAll(n => !IsValid(n));
            }
        }

        // 4. 유효성 검사 함수
        bool IsValid(StageNodeVer2 n)
        {
            var p = n.GridPosition;
            return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height && grid[p.x, p.y] != null;
        }
    }

    private HashSet<StageNodeVer2> CollectNodesReachableFrom(StageNodeVer2 end)
    {
        var visited = new HashSet<StageNodeVer2>();
        var stack = new Stack<StageNodeVer2>();
        stack.Push(end);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node == null || visited.Contains(node)) continue;

            visited.Add(node);

            foreach (var prev in node.IncomingNodes)
                stack.Push(prev);
        }

        return visited;
    }
    
    private HashSet<StageNodeVer2> CollectConnectedNodes(StageNodeVer2 start)
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

    private void GenerateNodeButtons()
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

                // ✅ 위치 캐싱 로직
                Vector2 offset;
                if (node.CachedPosition.HasValue)
                {
                    offset = node.CachedPosition.Value;
                }
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

    private void DrawConnections()
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

    void HighlightAvailableNodes(StageNodeVer2 current)
    {
        foreach (var pair in nodeButtons)
        {
            var node = pair.Key;
            var button = pair.Value;

            bool isReachable =
                current.ConnectedNodes.Contains(node) ||
                current.IncomingNodes.Contains(node) ||
                node.IncomingNodes.Contains(current);

            button.GetComponent<Button>().interactable = isReachable;
        }
    }

    public void OnNodeSelected(StageNodeVer2 selectedNode)
    {
        bool isReachable =
            currentNode != null &&
            (currentNode.ConnectedNodes.Contains(selectedNode) ||
             currentNode.IncomingNodes.Contains(selectedNode) ||
             selectedNode.IncomingNodes.Contains(currentNode));

        if (!isReachable)
            return;

        // ✅ 이동 (씬 전환 여부와 관계없이)
        currentNode = selectedNode;
        RuntimeDataManager.Instance.currentNode = currentNode;
        HighlightAvailableNodes(currentNode);

        UpdateMarkerPosition(currentNode);
        
        // ✅ 클리어된 노드일 경우 씬 전환 없이 종료
        if (selectedNode.IsCleared)
        {
            Debug.Log("이미 클리어한 노드에 도착함. 씬 전환 없음.");
            return;
        }

        // ⛳ 씬 전환 처리
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
    
    private void UpdateMarkerPosition(StageNodeVer2 node)
    {
        if (!nodeButtons.TryGetValue(node, out var button)) return;

        if (markerInstance == null)
        {
            markerInstance = Instantiate(markerPrefab, button.transform.parent);
        }

        markerInstance.SetActive(true);
        markerInstance.transform.SetAsLastSibling(); // 항상 위에 뜨게
        markerInstance.transform.position = button.transform.position;
    }
}