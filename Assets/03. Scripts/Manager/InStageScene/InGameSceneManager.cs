using System;
using Unity.VisualScripting;
using UnityEngine;

public class InGameSceneManager : MonoBehaviour
{
    public static InGameSceneManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }
    
    [Header("Manager 및 Spawner")]
    [SerializeField] public StageManager stageManager;
    [SerializeField] public AllyPoolManager allyPoolManager;
    [SerializeField] public DawnSpawnManager dawnSpawnManager;
    [SerializeField] public DarkSpawner darkSpawner;
    [SerializeField] public CardSpawner cardSpawner;
    [SerializeField] public TileManager tileManager;
    [SerializeField] public PreviewManager previewManager;
    

    private void Start()
    {
        // 현재 스테이지 데이터 존재 시 자동 시작
        if (RuntimeDataManager.Instance.currentStageData != null)
        {
            stageManager.StartStage();
        }
        else
        {
            Debug.LogWarning("currentStageData가 null입니다. MapScene에서 노드를 클릭하지 않았을 수 있습니다.");
        }
    }
}
