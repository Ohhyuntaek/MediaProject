using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// 스테이지 상태 (플레이 중, 클리어)
/// </summary>
public enum StageState
{
    Idle,
    Playing,
    Cleared
}

/// <summary>
/// 인게임 스테이지 전체를 관리하는 매니저
/// </summary>
public class StageManager : MonoBehaviour
{
    [FormerlySerializedAs("stageClearProcessed")]
    [Header("스테이지 관리 변수")] 
    [SerializeField] private bool stageCleared = false;
    [SerializeField] private StageState currentStageState = StageState.Idle;

    public StageState CurrentStageState => currentStageState;
    
    // ===========================================================
    // 초기화
    // ===========================================================

    private void Update()
    {
#if UNITY_EDITOR
        // 에디터용 테스트: F1 누르면 스테이지 강제 클리어
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("강제 스테이지 클리어 트리거");
            currentStageState = StageState.Cleared;
            InGameSceneManager.Instance.darkSpawner.StopSpawning();
            OnStageClear();
            return;
        }
#endif

        if (currentStageState != StageState.Playing)
            return;

        UIManager.Instance.inGameUIManager.UpdateHpSlider(InGameSceneManager.Instance.dawnSpawnManager.spawnedDawn);
        UIManager.Instance.inGameUIManager.UpdateEnergySlider(InGameSceneManager.Instance.dawnSpawnManager.spawnedDawn);
    }

    /// <summary>
    /// 스테이지 상태 설정 함수
    /// </summary>
    /// <param name="stageState"></param>
    public void SetStageState(StageState stageState)
    {
        currentStageState = stageState;
        switch (stageState)
        {
            case StageState.Idle:
            case StageState.Playing:
                stageCleared = false;
                break;
            case StageState.Cleared:
                stageCleared = true;
                break;
        }
    }
    
    /// <summary>
    /// 스테이지 시작 함수
    /// </summary>
    public void StartStage()
    {
        // 쿨다운 초기화
        if (GameManager.Instance.GetSelectedDawn() != null)
            GameManager.Instance.GetSelectedDawn().ResetActiveCooldown();
        
        SetStageState(StageState.Playing);
        
        // 총 코스트를 0으로 설정
        UIManager.Instance.costManager.TotalCost = 0;
        
        UIManager.Instance.costManager.StopCostUP(false);
       
      
        // 코스트 증가 코루틴 시작
        StartCoroutine(UIManager.Instance.costManager.IncreaseCost());
        
        InGameSceneManager.Instance.tileManager.PreviewAvailableTile();
        
        // 카드 스폰 시작
        InGameSceneManager.Instance.cardSpawner.CanSpawnCards = true;
        

        StageData stage = GameManager.Instance.currentStageData;
        if (stage.StageType == StageType.Normal)
        {
            SoundManager.Instance.PlayBgmList(1,true);
        }
        else if (stage.StageType == StageType.Boss)
        {
            SoundManager.Instance.PlayBgmList(2,true);
        }
        InGameSceneManager.Instance.darkSpawner.DarksCount = stage.DarksCount; // 매 스테이지마다 초기화
        UIManager.Instance.inGameUIManager.SetStageText(stage.StageName);

        InGameSceneManager.Instance.darkSpawner.StopSpawning(); // 이전 스폰 루틴 중지
        InGameSceneManager.Instance.darkSpawner.StartSpawning(stage);
    }
    
    /// <summary>
    /// 스테이지 클리어 함수
    /// </summary>
    public void OnStageClear()
    {
        SetStageState(StageState.Cleared);
        
        // 강화카드가 선택 중이면 클리어 처리 금지
        if (UIManager.Instance.clearUIManager.SpawnedEnhancementCards.Count > 0)
        {
            Debug.LogWarning("강화 카드 선택 중에 스테이지 클리어 호출됨 - 무시");
            return;
        }
        
        // 카드 스폰 정지
        InGameSceneManager.Instance.cardSpawner.CanSpawnCards = false;

        // 덱 카드 삭제
        InGameSceneManager.Instance.cardSpawner.ClearDeckCards();

        // 코스트 증가 중지
        UIManager.Instance.costManager.StopCostUP(true);
        //브금 정지
        //SoundManager.Instance.StopBgm();

        // 모든 Ally 제거
        foreach (var allyObj in InGameSceneManager.Instance.allyPoolManager.activateAllies)
        {
            if (allyObj != null)
            {
                var ally = allyObj.GetComponent<Ally>();
                if (ally != null)
                    ally.ForceDie();
            }
        }

        StartCoroutine(UIManager.Instance.clearUIManager.HandleStageClearSequence(GameManager.Instance.currentStageData.StageType));
    }

    // /// <summary>
    // /// 다음 스테이지로 넘어가는 함수
    // /// </summary>
    // public void NextStage()
    // {
    //     SetStageState(StageState.Playing);
    //     StartStage();
    // }
}
