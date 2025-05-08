using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스테이지 노드 클래스 정의
public class StageNode
{
    public int id;
    public StageType stageType;
    public List<StageNode> children = new();
    public int depth;
    public bool isCurrent = false;
}

public class StageTreeGenerator : MonoBehaviour
{
    [Header("그래프 생성 파라미터")]
    public int maxDepth = 5;
    public int maxNodesPerDepth = 4;
    public int minShopCount = 1;
    public int maxShopCount = 3;

    [Header("UI 구성 요소")]
    public GameObject nodePrefab;
    public RectTransform nodeRoot;
    public GameObject linePrefab;

    [Header("그래프 높이 설정")]
    public float totalHeight = 1000f;

    private List<StageNode> allNodes = new();
    private Dictionary<StageNode, Vector2> nodePositions = new();
    private Dictionary<StageNode, GameObject> nodeMap = new();
    private int idCounter = 0;
    private StageNode currentNode;
    private StageNode bossNode;

    void Start()
    {
        StageNode root = GenerateStageGraph();
        currentNode = root;
        currentNode.isCurrent = true;

        CalculateNodePositions();
        InstantiateNodes();
    }

    StageNode GenerateStageGraph()
    {
        allNodes.Clear();
        idCounter = 0;

        StageNode root = new StageNode { id = idCounter++, stageType = StageType.Normal, depth = 0 };
        allNodes.Add(root);

        GenerateSharedGraph(root);

        bossNode = new StageNode { id = idCounter++, stageType = StageType.Boss, depth = maxDepth };
        allNodes.Add(bossNode);
        ConnectLeavesToBoss();

        AssignShopStages();
        return root;
    }

    void GenerateSharedGraph(StageNode root)
    {
        List<StageNode> currentDepthNodes = new() { root };

        for (int depth = 1; depth < maxDepth; depth++)
        {
            List<StageNode> nextDepthNodes = new();
            Dictionary<StageNode, int> parentCountMap = new();

            int targetNodeCount = Mathf.Min(Random.Range(2, maxNodesPerDepth + 1), maxNodesPerDepth);

            for (int i = 0; i < targetNodeCount; i++)
            {
                var child = new StageNode
                {
                    id = idCounter++,
                    stageType = StageType.Normal,
                    depth = depth
                };
                allNodes.Add(child);
                nextDepthNodes.Add(child);
                parentCountMap[child] = 0;
            }

            foreach (var parent in currentDepthNodes)
            {
                int childLinkCount = Random.Range(1, 3);
                var selected = nextDepthNodes.OrderBy(_ => Random.value).Take(childLinkCount);

                foreach (var child in selected)
                {
                    parent.children.Add(child);
                    parentCountMap[child]++;
                }
            }

            foreach (var child in nextDepthNodes)
            {
                if (parentCountMap[child] == 0)
                {
                    var randomParent = currentDepthNodes[Random.Range(0, currentDepthNodes.Count)];
                    randomParent.children.Add(child);
                }
            }

            currentDepthNodes = nextDepthNodes;
        }
    }

    void ConnectLeavesToBoss()
    {
        foreach (var node in allNodes)
        {
            if (node.children.Count == 0 && node.depth == maxDepth - 1)
            {
                node.children.Add(bossNode);
            }
        }
    }

    void AssignShopStages()
    {
        var candidates = allNodes
            .Where(n => n.stageType == StageType.Normal)
            .OrderBy(n => n.depth)
            .ToList();

        int shopCount = Mathf.Min(Random.Range(minShopCount, maxShopCount + 1), candidates.Count);
        int attempts = 0;
        int maxAttempts = 100;

        while (shopCount > 0 && attempts < maxAttempts)
        {
            attempts++;
            var candidate = candidates[Random.Range(0, candidates.Count)];

            bool adjacentShopExists = false;

            var sameDepth = allNodes.Where(n => n.depth == candidate.depth).ToList();
            int index = sameDepth.IndexOf(candidate);

            if ((index > 0 && sameDepth[index - 1].stageType == StageType.Shop) ||
                (index < sameDepth.Count - 1 && sameDepth[index + 1].stageType == StageType.Shop))
            {
                adjacentShopExists = true;
            }

            bool parentIsShop = allNodes.Any(n => n.children.Contains(candidate) && n.stageType == StageType.Shop);
            bool childIsShop = candidate.children.Any(c => c.stageType == StageType.Shop);

            if (!adjacentShopExists && !parentIsShop && !childIsShop)
            {
                candidate.stageType = StageType.Shop;
                shopCount--;
                candidates.Remove(candidate);
            }
        }
    }

    void CalculateNodePositions()
    {
        Dictionary<int, List<StageNode>> depthMap = new();
        foreach (var node in allNodes)
        {
            if (!depthMap.ContainsKey(node.depth))
                depthMap[node.depth] = new List<StageNode>();
            depthMap[node.depth].Add(node);
        }

        float xSpacing = 250f;
        float ySpacing = -totalHeight / (maxDepth + 1);
        nodePositions.Clear();

        foreach (var pair in depthMap)
        {
            int depth = pair.Key;
            List<StageNode> nodes = pair.Value;

            float totalWidth = (nodes.Count - 1) * xSpacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < nodes.Count; i++)
            {
                float x = startX + i * xSpacing;
                float y = depth * ySpacing;
                nodePositions[nodes[i]] = new Vector2(x, y);
            }
        }
    }

    void InstantiateNodes()
    {
        nodeMap.Clear();

        foreach (var kvp in nodePositions)
        {
            var node = kvp.Key;
            var pos = kvp.Value;

            GameObject go = Instantiate(nodePrefab, nodeRoot);
            go.GetComponent<RectTransform>().anchoredPosition = pos;

            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = node.stageType.ToString();

            var img = go.GetComponentInChildren<Image>();
            img.color = GetVisualColor(node);

            nodeMap[node] = go;

            var button = go.transform.Find("Button")?.GetComponent<Button>();
            if (button != null)
            {
                var capturedNode = node;
                Debug.Log($"버튼 연결: {capturedNode.id}");
                button.onClick.AddListener(() => OnNodeClicked(capturedNode));
            }
            else
            {
                Debug.LogWarning("버튼이 연결되지 않았습니다.");
            }
        }

        foreach (var kvp in nodeMap)
        {
            var parentNode = kvp.Key;
            var parentGO = kvp.Value;

            foreach (var child in parentNode.children)
            {
                Vector2 start = parentGO.GetComponent<RectTransform>().anchoredPosition;
                Vector2 end = nodeMap[child].GetComponent<RectTransform>().anchoredPosition;
                DrawLine(start, end);
            }
        }
    }

    void OnNodeClicked(StageNode clickedNode)
    {
        if (currentNode.children.Contains(clickedNode))
        {
            currentNode.isCurrent = false;
            currentNode = clickedNode;
            currentNode.isCurrent = true;

            foreach (var node in allNodes)
            {
                var img = nodeMap[node].GetComponentInChildren<Image>();
                img.color = GetVisualColor(node);
            }

            Debug.Log($"이동: {clickedNode.id}, 타입: {clickedNode.stageType}");
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        var line = Instantiate(linePrefab, nodeRoot);
        RectTransform rt = line.GetComponent<RectTransform>();
        Vector2 direction = end - start;
        float length = direction.magnitude;
        rt.sizeDelta = new Vector2(length, 5f);
        rt.anchoredPosition = start + direction / 2f;
        rt.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    Color GetVisualColor(StageNode node)
    {
        if (node.isCurrent) return Color.green;
        if (currentNode != null && currentNode.children.Contains(node)) return Color.magenta;
        return GetColor(node.stageType);
    }

    Color GetColor(StageType type)
    {
        return type switch
        {
            StageType.Normal => Color.cyan,
            StageType.Shop => Color.yellow,
            StageType.Boss => Color.red,
            _ => Color.white,
        };
    }
}
