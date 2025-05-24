using UnityEngine;
using UnityEngine.UI;

// UI 버튼 노드에 붙는 스크립트
public class NodeButton : MonoBehaviour
{
    private StageNodeVer2 stageNode;
    private MapGenerator mapGenerator;

    public Button button;
    
    // 노드 초기화 함수
    public void Init(StageNodeVer2 node)
    {
        stageNode = node;
        mapGenerator = FindObjectOfType<MapGenerator>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    public void OnClick()
    {
        mapGenerator.OnNodeSelected(stageNode);
    }
}