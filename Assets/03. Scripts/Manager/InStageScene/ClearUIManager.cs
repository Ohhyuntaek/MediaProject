using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ClearUIManager : MonoBehaviour
{
    [SerializeField, LabelText("스테이지 클리어 텍스트")] private TMP_Text stageClearText;
    
    [Header("클리어 메달 관련")]
    [SerializeField, LabelText("클리어 메달 애니메이터")]
    private Animator clearMedalAnimator;
    [SerializeField, LabelText("클리어 메달과 클리어 텍스트 사이의 시간 간격")] 
    private float clearMedalAndTextTerm = 3f;
    
    [Header("강화 카드 관련")]
    [SerializeField] private List<EnhancementCardData> enhancementCardDataList;
    [SerializeField] private List<GameObject> enhancementCardPrefabs;
    [SerializeField] private List<Transform> enhancementCardSlots;
    
    private List<GameObject> spawnedEnhancementCards = new(); // 생성된 강화 카드 리스트
    
    public List<GameObject> SpawnedEnhancementCards => spawnedEnhancementCards;

    private void Start()
    {
        SetActiveStageClearText(false);
    }

    /// <summary>
    /// 1. 클리어 메달 내려오기
    /// 2. clearMedalAndTextTerm만큼이 지난 후 클리어 텍스트 표시
    /// 3. 스테이지 타입에 따라 강화 카드 보이기 여부 확인
    ///     3-1. Normal 스테이지 => 강화 카드 보이기
    ///     3-2. Boss 스테이지 => Main Scene으로 이동
    /// </summary>
    /// <param name="stageType"></param>
    /// <returns></returns>
    public IEnumerator HandleStageClearSequence(StageType stageType)
    {
        if (clearMedalAnimator != null)
        {
            clearMedalAnimator.SetTrigger("isCleared");
            yield return new WaitUntil(() => clearMedalAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClearMedalDown"));
        }

        SetActiveStageClearText(true, "Stage Clear!");

        yield return new WaitForSeconds(clearMedalAndTextTerm);

        SetActiveStageClearText(false);

        if (stageType == StageType.Normal)
        {
            ShowEnhancementCard();
        }
        else if (stageType == StageType.Boss)
        {
            // SceneManager.LoadScene("MainScene");
            RuntimeDataManager.Instance.itemCollector.ClearAll();
            LoadingSceneManager.LoadScene("MainScene");
        }
    }
    
    /// <summary>
    /// 강화 카드를 보여주는 함수
    /// </summary>
    private void ShowEnhancementCard()
    {
        List<EnhancementCardData> candidates = new(enhancementCardDataList);

        for (int i = 0; i < 3; i++)
        {
            int dataIndex = Random.Range(0, candidates.Count);
            EnhancementCardData selectedData = candidates[dataIndex];
            candidates.RemoveAt(dataIndex);

            int prefabIndex = Random.Range(0, enhancementCardPrefabs.Count);
            GameObject selectedPrefab = enhancementCardPrefabs[prefabIndex];

            GameObject cardObj = Instantiate(selectedPrefab, enhancementCardSlots[i]);
            cardObj.transform.localPosition = Vector3.zero;
            cardObj.transform.localScale = Vector3.one;

            EnhancementCard card = cardObj.GetComponent<EnhancementCard>();
            if (card != null)
                card.Setup(selectedData);

            spawnedEnhancementCards.Add(cardObj);
        }
    }
    
    /// <summary>
    /// 강화 카드 선택 후, 강화 카드 숨기기 및 ProceedToNextStageAfterDelay 코루틴 시작
    /// </summary>
    public void HideEnhancementCardsAndProceed()
    {
        foreach (var card in spawnedEnhancementCards)
            Destroy(card);

        spawnedEnhancementCards.Clear();
        
        StartCoroutine(ProceedToNextStageAfterDelay());
    }
    
    /// <summary>
    /// 메달 올라가기 애니메이션 진행 및 3초 대기
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProceedToNextStageAfterDelay()
    {
        clearMedalAnimator.SetTrigger("isPlaying");
        yield return new WaitForSeconds(1f);
        // InGameSceneManager.Instance.stageManager.NextStage();
        OnReturnToMap();
    }
    
    /// <summary>
    /// 클리어 텍스트 온오프
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveStageClearText(bool active, string clearText = "None")
    {
        stageClearText.gameObject.SetActive(active);

        if (active)
        {
            stageClearText.text = clearText;
        }
    }
    
    /// <summary>
    /// 다시 MapScene으로 돌아가기
    /// </summary>
    public void OnReturnToMap()
    {
        RuntimeDataManager.Instance.lumenCalculator.AddLumen(10);
        
        // SceneManager.LoadScene("MapScene");
        LoadingSceneManager.LoadScene("MapScene");
    }
}
