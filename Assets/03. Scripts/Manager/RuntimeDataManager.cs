using System.Collections.Generic;
using UnityEngine;

public class RuntimeDataManager : MonoBehaviour
{
    public static RuntimeDataManager Instance;
    
    [Header("선택한 Dawn 캐릭터")]
    [SerializeField] private Dawn selectedDawn;
    
    [Header("맵")]
    public List<StageNode> stageGraphData = new();
    
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
