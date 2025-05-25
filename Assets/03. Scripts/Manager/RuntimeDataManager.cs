using System.Collections.Generic;
using UnityEngine;

public class RuntimeDataManager : MonoBehaviour
{
    public static RuntimeDataManager Instance;
    
    [Header("선택한 Dawn 캐릭터")]
    [SerializeField] private Dawn selectedDawn;
    
    [Header("맵 상태")]
    public StageNodeVer2[,] mapGrid;                // 전체 맵 구조
    public StageNodeVer2 currentNode;               // 현재 노드
    public StageNodeVer2 nextNode;                  // 다음으로 이동할 노드
    public bool mapGenerated = false;               // 맵 생성 여부
    
    [Header("현재 스테이지")]
    public StageData currentStageData;
    
    [Header("진행 노드 ID")]
    public int currentStageNodeId = -1;

    [Header("강화 효과")]
    public Enhancement enhancement;
    
    [Header("재화 관리 및 계산")]
    public LumenCalculator lumenCalculator;
    
    [Header("수집한 아이템")]
    public ItemCollector itemCollector;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 플레이어 선택할 때 호출
    public void SetSelectedDawn(Dawn dawn)
    {
        selectedDawn = dawn;
    }
    
    public Dawn GetSelectedDawn()
    {
        return selectedDawn;
    }
}
