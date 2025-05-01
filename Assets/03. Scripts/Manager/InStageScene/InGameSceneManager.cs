using System;
using UnityEngine;

public class InGameSceneManager : MonoBehaviour
{
    public static InGameSceneManager Instance;

    [Header("Manager 및 Spawner")]
    [SerializeField] public StageManager stageManager;
    [SerializeField] public InGameUIManager inGameUIManager;
    [SerializeField] public ClearUIManager clearUIManager;
    [SerializeField] public AllyPoolManager allyPoolManager;
    [SerializeField] public CostManager costManager;
    [SerializeField] public DawnSpawnManager dawnSpawnManager;
    [SerializeField] public DarkSpawner darkSpawner;
    [SerializeField] public CardSpawner cardSpawner;
    [SerializeField] public TileManager tileManager;
    
    private void Awake()
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

    private void Start()
    {
        inGameUIManager.SetActiveStageClearText(false);
        inGameUIManager.UpdateDawnImage(GameManager.Instance.GetSelectedDawn().DawnData);
        inGameUIManager.SetDawnCoolTimeProgress(GameManager.Instance.GetSelectedDawn());
        
        stageManager.StartStage();
    }
}
