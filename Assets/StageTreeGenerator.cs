using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// 각 스테이지 노드를 표현하는 클래스
public class StageNode
{
    public int id;  // 노드 고유 ID
    public StageType stageType;  // 스테이지 타입 (Normal, Shop, Boss)
    public List<StageNode> children = new();  // 연결된 자식 노드들
    public int depth;  // 노드의 깊이 (단계)
    public bool isCurrent = false;  // 현재 위치한 노드인지 여부
    public StageData dataAsset;  // 연결된 스테이지 데이터 (ScriptableObject)
}

// 트리 구조의 스테이지를 생성하는 메인 클래스
public class StageTreeGenerator : MonoBehaviour
{
    [Header("그래프 생성 파라미터")]
    public int maxDepth = 5;  // 그래프의 최대 깊이
    public int maxNodesPerDepth = 4;  // 각 깊이에 생성할 최대 노드 수
    public int minShopCount = 1;  // 최소 상점 노드 수
    public int maxShopCount = 3;  // 최대 상점 노드 수

    [Header("UI 구성 요소")]
    public GameObject nodePrefab;  // 노드 UI 프리팹
    public RectTransform nodeRoot;  // 노드들이 배치될 UI 부모
    public GameObject linePrefab;  // 노드 간 경로를 그릴 프리팹

    [Header("스테이지 타입별 스프라이트")]
    public Sprite normalSprite;
    public Sprite shopSprite;
    public Sprite bossSprite;

    [Header("스테이지 데이터")]
    public List<StageData> normalStageDataList;  // Normal 노드에 할당될 데이터 리스트
    public StageData bossStageData;  // Boss 노드 데이터
    public List<StageData> shopStageDataList;  // Shop 노드에 할당될 데이터 리스트

    [Header("Path Sprite 설정")]
    public Sprite defaultPathSprite;
    public Sprite highlightedPathSprite;

    [Header("Path 설정")]
    public float nodeRadius = 50f;  // 노드 경계로부터 경로 시작점 거리
    public float pathThickness = 30f;  // 경로의 두께

    [Header("그래프 높이 설정")]
    public float totalHeight = 1000f;  // 전체 그래프 높이

    private List<StageNode> allNodes = new();  // 모든 노드 리스트
    private Dictionary<StageNode, Vector2> nodePositions = new();  // 노드별 위치
    private Dictionary<StageNode, GameObject> nodeMap = new();  // 노드와 UI 매핑
    private Dictionary<(StageNode, StageNode), Image> pathMap = new();  // 경로와 이미지 매핑
    private StageNode currentNode;  // 현재 위치 노드
    private StageNode bossNode;  // 보스 노드 참조
    private int idCounter = 0;  // 노드 ID 카운터
    private int normalStageIndex = 0;  // Normal Stage 순환 인덱스

    void Start()
    {
        StageNode root = GenerateStageGraph();  // 그래프 생성
        
        int storedId = GameManager.Instance.currentStageNodeId;

        if (storedId != -1)  // 저장된 위치가 있으면 복원
        {
            currentNode = allNodes.FirstOrDefault(n => n.id == storedId);
            if (currentNode != null)
                currentNode.isCurrent = true;
        }
        else
        {
            currentNode = null;
        }
        
        CalculateNodePositions();  // 노드 위치 계산
        InstantiateNodes();  // 노드 및 경로 UI 생성
    }

    // 전체 그래프 구성
    StageNode GenerateStageGraph()
    {
        allNodes.Clear();
        idCounter = 0;

        // 루트 노드 생성 및 데이터 연결
        StageNode root = new StageNode
        {
            id = idCounter++,
            stageType = StageType.Normal,
            depth = 0,
            dataAsset = normalStageDataList[normalStageIndex++ % normalStageDataList.Count]
        };
        allNodes.Add(root);

        GenerateSharedGraph(root);  // 중간 노드 생성 및 연결

        // 보스 노드 생성 및 데이터 연결
        bossNode = new StageNode
        {
            id = idCounter++,
            stageType = StageType.Boss,
            depth = maxDepth,
            dataAsset = bossStageData
        };
        allNodes.Add(bossNode);

        ConnectLeavesToBoss();  // 마지막 노드와 보스 연결
        AssignShopStages();  // 상점 노드 지정

        return root;
    }

    // 중간 노드 생성 및 연결
    void GenerateSharedGraph(StageNode root)
    {
        List<StageNode> currentDepthNodes = new() { root };

        for (int depth = 1; depth < maxDepth; depth++)
        {
            List<StageNode> nextDepthNodes = new();
            Dictionary<StageNode, int> parentCountMap = new();

            int targetCount = Mathf.Min(Random.Range(2, maxNodesPerDepth + 1), maxNodesPerDepth);

            // 노드 생성 및 데이터 연결
            for (int i = 0; i < targetCount; i++)
            {
                StageNode child = new StageNode
                {
                    id = idCounter++,
                    stageType = StageType.Normal,
                    depth = depth,
                    dataAsset = normalStageDataList[normalStageIndex++ % normalStageDataList.Count]
                };
                allNodes.Add(child);
                nextDepthNodes.Add(child);
                parentCountMap[child] = 0;
            }

            // 랜덤 연결
            foreach (var parent in currentDepthNodes)
            {
                var selected = nextDepthNodes.OrderBy(_ => Random.value).Take(Random.Range(1, 3));
                foreach (var child in selected)
                {
                    parent.children.Add(child);
                    parentCountMap[child]++;
                }
            }

            // 고아 노드 연결 보정
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

    // 리프 노드를 보스 노드와 연결
    void ConnectLeavesToBoss()
    {
        foreach (var node in allNodes)
        {
            if (node.children.Count == 0 && node.depth == maxDepth - 1)
                node.children.Add(bossNode);
        }
    }

    // 상점 노드 지정
    void AssignShopStages()
    {
        var candidates = allNodes.Where(n => n.stageType == StageType.Normal && n.depth > 0).ToList();
        int shopCount = Mathf.Min(Random.Range(minShopCount, maxShopCount + 1), candidates.Count);

        int attempts = 0;
        while (shopCount > 0 && attempts++ < 100)
        {
            var candidate = candidates[Random.Range(0, candidates.Count)];

            if (candidate.stageType == StageType.Shop) continue;

            // 조건 검사
            bool conflict = allNodes.Any(n => n.children.Contains(candidate) && n.stageType == StageType.Shop)
                          || candidate.children.Any(c => c.stageType == StageType.Shop);

            if (!conflict)
            {
                candidate.stageType = StageType.Shop;
                candidate.dataAsset = shopStageDataList[Random.Range(0, shopStageDataList.Count)];
                shopCount--;
            }
        }
    }

    // UI용 노드 위치 계산
    void CalculateNodePositions()
    {
        float xSpacing = 250f;
        float ySpacing = -totalHeight / (maxDepth + 1);

        var depthMap = allNodes.GroupBy(n => n.depth).ToDictionary(g => g.Key, g => g.ToList());
        nodePositions.Clear();

        foreach (var pair in depthMap)
        {
            float totalWidth = (pair.Value.Count - 1) * xSpacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < pair.Value.Count; i++)
            {
                nodePositions[pair.Value[i]] = new Vector2(startX + i * xSpacing, pair.Key * ySpacing);
            }
        }
    }

    // 노드 및 UI 배치
    void InstantiateNodes()
    {
        nodeMap.Clear();

        foreach (var pair in nodePositions)
        {
            var node = pair.Key;
            var pos = pair.Value;
            var go = Instantiate(nodePrefab, nodeRoot);
            go.GetComponent<RectTransform>().anchoredPosition = pos;

            var img = go.GetComponentInChildren<Image>();
            img.sprite = GetSpriteByStageType(node.stageType);

            var highlight = go.transform.Find("Highlighter")?.gameObject;
            if (highlight != null) highlight.SetActive(node.isCurrent);

            var btn = go.transform.Find("Button")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnNodeClicked(node));
            }

            nodeMap[node] = go;
        }

        foreach (var parent in nodeMap.Keys)
        {
            foreach (var child in parent.children)
            {
                var start = nodeMap[parent].GetComponent<RectTransform>().anchoredPosition;
                var end = nodeMap[child].GetComponent<RectTransform>().anchoredPosition;
                DrawLine(start, end, parent, child);
            }
        }
    }

    // 노드 클릭 처리
    void OnNodeClicked(StageNode clickedNode)
    {
        if ((currentNode == null && clickedNode.depth != 0) || (currentNode != null && !currentNode.children.Contains(clickedNode)))
            return;

        currentNode = clickedNode;
        currentNode.isCurrent = true;

        GameManager.Instance.currentStageData = clickedNode.dataAsset;
        GameManager.Instance.currentStageNodeId = clickedNode.id;  // 현재 노드 ID 저장


        foreach (var node in allNodes)
        {
            var go = nodeMap[node];
            go.transform.Find("Highlighter")?.gameObject.SetActive(node == currentNode);
            go.GetComponentInChildren<Image>().sprite = GetSpriteByStageType(node.stageType);
        }

        foreach (var kv in pathMap)
        {
            kv.Value.sprite = (kv.Key.Item1 == currentNode) ? highlightedPathSprite : defaultPathSprite;
        }

        Debug.Log($"이동: {clickedNode.id}, 타입: {clickedNode.stageType}");
        
        // 씬 전환
        SceneManager.LoadScene("InStage");
    }

    // 두 노드 간 선 그리기
    void DrawLine(Vector2 start, Vector2 end, StageNode parent, StageNode child)
    {
        var line = Instantiate(linePrefab, nodeRoot);
        var rt = line.GetComponent<RectTransform>();

        Vector2 dir = (end - start).normalized;
        Vector2 s = start + dir * nodeRadius;
        Vector2 e = end - dir * nodeRadius;
        Vector2 d = e - s;

        rt.sizeDelta = new Vector2(d.magnitude, pathThickness);
        rt.anchoredPosition = s + d / 2;
        rt.rotation = Quaternion.FromToRotation(Vector3.right, d);

        var img = line.GetComponent<Image>();
        img.sprite = defaultPathSprite;
        img.preserveAspect = true;
        pathMap[(parent, child)] = img;
    }

    // 타입별 스프라이트 반환
    Sprite GetSpriteByStageType(StageType type) => type switch
    {
        StageType.Normal => normalSprite,
        StageType.Shop => shopSprite,
        StageType.Boss => bossSprite,
        _ => null
    };
}
