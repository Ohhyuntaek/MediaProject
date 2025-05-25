using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI 버튼 노드에 붙는 스크립트
public class NodeButton : MonoBehaviour
{
    private StageNodeVer2 stageNode;
    private MapGenerator mapGenerator;

    [Header("시작 노드용 설정")]
    public Sprite homeSprite;  // 시작 노드 전용 아이콘
    public string homeLabel = "Home";
    
    [Header("타입별 이미지")]
    public Sprite normalSprite;
    public Sprite shopSprite;
    public Sprite bossSprite;
    
    private Image backgroundImage;
    public Button button;
    public TMP_Text label;
    public Image clearedHighlight;
    
    // 노드 초기화 함수
    public void Init(StageNodeVer2 node)
    {
        stageNode = node;
        mapGenerator = FindObjectOfType<MapGenerator>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        
        backgroundImage = GetComponent<Image>(); // 배경 이미지 받아오기
        
        Vector2Int pos = stageNode.GridPosition;
        bool isStartNode = (pos.x == 0 && pos.y == 0);
        
        if (isStartNode)
        {
            // 시작 노드인 경우 특별한 처리
            if (label != null)
            {
                label.text = homeLabel;
            }

            if (backgroundImage != null && homeSprite != null)
                backgroundImage.sprite = homeSprite;
            
            clearedHighlight.gameObject.SetActive(false);
        }
        else if (stageNode.StageData != null)
        {
            label.text = stageNode.StageData.StageType.ToString();

            if (backgroundImage != null)
            {
                switch (stageNode.StageData.StageType)
                {
                    case StageType.Normal:
                        backgroundImage.sprite = normalSprite;
                        break;
                    case StageType.Shop:
                        backgroundImage.sprite = shopSprite;
                        break;
                    case StageType.Boss:
                        backgroundImage.sprite = bossSprite;
                        break;
                }
            }
        }
        
        if (clearedHighlight != null)
        {
            // 시작 노드는 항상 비활성화
            clearedHighlight.gameObject.SetActive(!isStartNode && stageNode.IsCleared);
        }
    }
    
    public void OnClick()
    {
        mapGenerator.OnNodeSelected(stageNode);
    }
    
    public void Refresh()
    {
        if (clearedHighlight != null)
        {
            Vector2Int pos = stageNode.GridPosition;
            bool isStartNode = (pos.x == 0 && pos.y == 0);

            clearedHighlight.gameObject.SetActive(!isStartNode && stageNode.IsCleared);
        }
    }
}