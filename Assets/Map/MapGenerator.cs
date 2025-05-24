using System.Collections.Generic;
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

    private StageNodeVer2[,] grid;
    private Dictionary<StageNodeVer2, GameObject> nodeButtons = new();
    private StageNodeVer2 currentNode;

    void Start()
    {
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

            // 💡 Boss 노드까지 경로가 이어질 때까지 반복 생성
            do
            {
                GenerateLogicalGrid();

                // 💡 보스 노드 강제 생성
                if (grid[width - 1, height - 1] == null)
                    grid[width - 1, height - 1] = new StageNodeVer2(width - 1, height - 1);

                bossNode = grid[width - 1, height - 1]; // 보스 노드 캐싱

                AssignStageTypes();
                ConnectNodes();

                var reachable = CollectConnectedNodes(grid[0, 0]);

                // 💥 Boss까지 도달 불가능하면 다시 생성
                if (!reachable.Contains(bossNode))
                    continue;

                RemoveDisconnectedNodes();
                ConnectNodes();
                PruneUnreachableNodes();
                ConnectNodes();

                break;

            } while (true);

            currentNode = grid[0, 0];
            SaveMapToRuntime();
        }

        GenerateNodeButtons();
        HighlightAvailableNodes(currentNode);
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

    private void GenerateLogicalGrid()
    {
        grid = new StageNodeVer2[width, height];
        int x = 0, y = 0;
        grid[x, y] = new StageNodeVer2(x, y);

        while (x < width - 1 || y < height - 1)
        {
            bool canRight = x < width - 1;
            bool canUp = y < height - 1;
            bool moveRight = canRight && (!canUp || Random.value < 0.5f);

            if (moveRight) x++;
            else y++;

            if (grid[x, y] == null)
                grid[x, y] = new StageNodeVer2(x, y);
        }

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (grid[i, j] == null && Random.value < nodeSpawnChance)
                {
                    grid[i, j] = new StageNodeVer2(i, j);
                }
            }
        }
        
        if (grid[width - 1, height - 1] == null)
        {
            grid[width - 1, height - 1] = new StageNodeVer2(width - 1, height - 1);
        }
    }

    private void AssignStageTypes()
    {
        int shopCount = 0;
        int maxShops = 3;

        for (int y = 0; y < height; y++)
        {
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
        }

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
        int[,] inDegree = new int[width, height];

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                foreach (var target in node.ConnectedNodes)
                {
                    Vector2Int pos = target.GridPosition;
                    inDegree[pos.x, pos.y]++;
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                bool isStart = (x == 0 && y == 0);
                bool isEnd = (x == width - 1 && y == height - 1);

                bool hasIn = isStart || inDegree[x, y] > 0;
                bool hasOut = isEnd || node.ConnectedNodes.Count > 0;

                if (!hasIn || !hasOut)
                {
                    grid[x, y] = null;
                }
            }
        }
    }

    private void RemoveDisconnectedNodes()
    {
        var reachable = CollectConnectedNodes(grid[0, 0]);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var node = grid[x, y];
                if (node == null) continue;

                if (!reachable.Contains(node))
                {
                    grid[x, y] = null;
                }
            }
        }
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
}