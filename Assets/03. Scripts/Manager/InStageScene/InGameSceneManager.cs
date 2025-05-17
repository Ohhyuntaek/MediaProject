using System;
using Unity.VisualScripting;
using UnityEngine;

public class InGameSceneManager : MonoBehaviour
{
    private static InGameSceneManager _instance;

    public static InGameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load(nameof(InGameSceneManager))).GetComponent<InGameSceneManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    [Header("Manager 및 Spawner")]
    [SerializeField] public StageManager stageManager;
    
    [SerializeField] public AllyPoolManager allyPoolManager;
    [SerializeField] public CostManager costManager;
    [SerializeField] public DawnSpawnManager dawnSpawnManager;
    [SerializeField] public DarkSpawner darkSpawner;
    [SerializeField] public CardSpawner cardSpawner;
    [SerializeField] public TileManager tileManager;
    

    private void Start()
    {
        // 현재 스테이지 데이터 존재 시 자동 시작
        if (GameManager.Instance.currentStageData != null)
        {
            stageManager.StartStage();
        }
        else
        {
            Debug.LogWarning("currentStageData가 null입니다. MapScene에서 노드를 클릭하지 않았을 수 있습니다.");
        }
    }
}
