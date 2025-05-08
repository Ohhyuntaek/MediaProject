using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

// 스테이지 노드 클래스 정의
public class StageNode
{
    public int id; // 고유 ID
    public StageType stageType; // 스테이지 타입 (Normal, Shop, Boss)
    public List<StageNode> children = new(); // 자식 노드 리스트 (다음으로 연결될 노드들)
    public int depth; // 스테이지 깊이 (UI 배치와 논리 흐름 판단에 사용)
    public bool isCurrent = false; // 현재 위치한 노드 여부
}

public class StageTreeGenerator : MonoBehaviour
{
    [Header("그래프 생성 파라미터")]
    public int maxDepth = 5;
    public int maxNodesPerDepth = 4;
    public int minShopCount = 1;
    public int maxShopCount = 3;

    [Header("UI 구성 요소")]
    public GameObject nodePrefab; // 노드 UI 프리팹
    public RectTransform nodeRoot; // 노드들이 위치할 UI 부모
    public GameObject linePrefab; // 노드 간 연결선을 나타낼 UI 프리팹

    [Header("그래프 높이 설정")]
    public float totalHeight = 1000f; // 전체 그래프 높이 (ySpacing 계산용)

    private List<StageNode> allNodes = new(); // 생성된 모든 노드 리스트
    private Dictionary<StageNode, Vector2> nodePositions = new(); // 노드 위치 정보
    private Dictionary<StageNode, GameObject> nodeMap = new(); // 노드와 UI GameObject 매핑
    private int idCounter = 0; // 고유 ID 부여용 카운터
    private StageNode currentNode; // 현재 선택된 노드
    private StageNode bossNode; // 마지막 보스 노드 참조

    // 게임 시작 시 실행되는 초기화 함수
    void Start()
    {
        StageNode root = GenerateStageGraph(); // 그래프 생성
        currentNode = null; // 시작 시 선택된 노드 없음 (루트 노드 클릭 유도)

        CalculateNodePositions(); // 노드 UI 위치 계산
        InstantiateNodes(); // 노드 UI 인스턴스화 및 연결선 생성
    }

    // 전체 그래프 생성: 루트 → 내부 노드 → 보스 노드 연결 → 상점 배치
    StageNode GenerateStageGraph()
    {
        allNodes.Clear();
        idCounter = 0;

        StageNode root = new StageNode { id = idCounter++, stageType = StageType.Normal, depth = 0 };
        allNodes.Add(root);

        GenerateSharedGraph(root); // 깊이 기반 그래프 생성

        bossNode = new StageNode { id = idCounter++, stageType = StageType.Boss, depth = maxDepth };
        allNodes.Add(bossNode);
        ConnectLeavesToBoss(); // 마지막 노드들을 보스에 연결

        AssignShopStages(); // 상점 노드 랜덤 배치
        return root;
    }

    // 깊이별로 노드를 생성하고 부모 노드와 연결하는 그래프 생성 함수
    void GenerateSharedGraph(StageNode root)
    {
        List<StageNode> currentDepthNodes = new() { root };

        for (int depth = 1; depth < maxDepth; depth++)
        {
            List<StageNode> nextDepthNodes = new();
            Dictionary<StageNode, int> parentCountMap = new();

            int targetNodeCount = Mathf.Min(Random.Range(2, maxNodesPerDepth + 1), maxNodesPerDepth);

            // 1. 자식 노드 생성
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

            // 2. 부모가 자식 노드와 무작위 연결
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

            // 3. 연결되지 않은 자식 노드는 랜덤 부모에게 강제 연결
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

    // 마지막 depth - 1 노드들을 보스 노드에 연결
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

    // 상점 노드를 랜덤하게 지정하되, 조건을 만족하지 않는 노드는 제외
    void AssignShopStages()
    {
        var candidates = allNodes
            .Where(n => n.stageType == StageType.Normal && n.depth > 0) // 루트 노드는 제외
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

    // 노드별 위치 계산 (depth 기반으로 수직, 개수 기반으로 수평 정렬)
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

    // 노드를 생성하고 UI에 배치하며, 클릭 이벤트 연결 및 선 그리기
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

        // 노드 간 연결선 그리기
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

    // 노드 클릭 처리: 루트만 처음 클릭 가능, 이후 연결된 노드만 이동 가능
    void OnNodeClicked(StageNode clickedNode)
    {
        if (currentNode == null)
        {
            if (clickedNode.depth != 0) return; // 루트만 클릭 허용
        }
        else
        {
            if (!currentNode.children.Contains(clickedNode)) return; // 자식 노드만 이동 허용
        }

        currentNode = clickedNode;
        currentNode.isCurrent = true;

        foreach (var node in allNodes)
        {
            var img = nodeMap[node].GetComponentInChildren<Image>();
            img.color = GetVisualColor(node);
        }

        Debug.Log($"이동: {clickedNode.id}, 타입: {clickedNode.stageType}");
    }

    // 두 노드를 선으로 연결하는 함수
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

    // 노드의 색상 결정: 현재 노드, 다음 가능 노드, 기본 색상 구분
    Color GetVisualColor(StageNode node)
    {
        if (node.isCurrent) return Color.green;
        if (currentNode == null && node.depth == 0) return Color.magenta;
        if (currentNode != null && currentNode.children.Contains(node)) return Color.magenta;
        return GetColor(node.stageType);
    }

    // 스테이지 타입별 색상 반환
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