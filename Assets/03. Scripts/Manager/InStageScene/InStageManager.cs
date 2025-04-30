using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// AllyType과 UnitData를 연결하는 데이터 클래스
/// </summary>
[System.Serializable]
public class AllyUnitDataLink
{
    public AllyType allyType;
    public UnitData unitData;
}

/// <summary>
/// 스테이지 상태 (플레이 중, 클리어)
/// </summary>
public enum StageState
{
    Playing,
    Cleared
}

/// <summary>
/// 인게임 스테이지 전체를 관리하는 매니저
/// </summary>
public class InStageManager : MonoBehaviour
{
    public static InStageManager Instance;

    [Header("카드 슬롯 관련")]
    [SerializeField] private Transform[] cardSlots;

    [Header("스테이지 데이터")]
    [SerializeField] private List<StageData> stageList;
    [SerializeField] private List<AllyUnitDataLink> allyUnitDataLinks;

    [Header("UI")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text stageClearText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image dawnImage;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider energySlider;

    [Header("게임 오브젝트")]
    [SerializeField] private DarkSpawner darkSpawner;
    [SerializeField] private CardSpawner cardSpawner;
    [SerializeField] private Transform dawnSpawnPoint;
    [SerializeField] private Animator clearMedalAnimator;

    [Header("강화 카드 관련")]
    [SerializeField] private List<EnhancementCardData> enhancementCardDataList;
    [SerializeField] private List<GameObject> enhancementCardPrefabs;
    [SerializeField] private List<Transform> enhancementCardSlots;

    [Header("기타 테스트 변수")] 
    [SerializeField] private bool stageClearProcessed = false;
    [SerializeField] private StageState currentStageState = StageState.Playing;
    [SerializeField] private int currentStageIndex = 0;   // 현재 스테이지 번호
    [SerializeField] private int stageDarksCount = 0;     // 현재 스테이지에서 남은 Dark 수
    [SerializeField] private float clearMedalAndTextTerm = 3f;
    [SerializeField] private float costUpMultiplier = 1.0f; // 코스트 증가 배율

    private List<GameObject> spawnedEnhancementCards = new(); // 생성된 강화 카드 리스트
    private Dictionary<AllyType, UnitData> allyUnitDataDict = new(); // AllyType과 UnitData 매칭

    private int cost = 0;               // 현재 코스트
    private float costTimer = 0f;        // 코스트 타이머
    private float costUpInterval => 1.0f / costUpMultiplier; // 코스트 증가 주기 계산식

    [SerializeField]
    private Dawn spawnedDawn;            // 생성된 Dawn 캐릭터


    // ===========================================================
    // 초기화
    // ===========================================================

    private void Awake()
    {
        Instance = this;

        // AllyType - UnitData 연결 매핑
        foreach (var link in allyUnitDataLinks)
        {
            if (!allyUnitDataDict.ContainsKey(link.allyType))
                allyUnitDataDict.Add(link.allyType, link.unitData);
            else
                Debug.LogWarning($"중복된 AllyType 존재: {link.allyType}");
        }
    }

    private void Start()
    {
        stageClearText.gameObject.SetActive(false);

        var selectedDawn = GameManager.Instance.GetSelectedDawn();
        if (selectedDawn == null)
        {
            Debug.LogError("GameManager에서 DawnData를 가져올 수 없습니다!");
            return;
        }

        if (dawnImage != null)
            dawnImage.sprite = selectedDawn.Portrait;
        
        SpawnDawn();

        StartStage();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // 에디터용 테스트: F1 누르면 스테이지 강제 클리어
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("강제 스테이지 클리어 트리거");
            stageDarksCount = 0;
            currentStageState = StageState.Cleared;
            OnStageClear();
            return;
        }
#endif

        if (currentStageState != StageState.Playing)
            return;

        // 코스트 증가 관리
        costTimer += Time.deltaTime;
        if (costTimer >= costUpInterval)
        {
            TryIncreaseCost();
            costTimer = 0f;
        }

        UpdateDawnUI();
    }

    // ===========================================================
    // Dawn 관련
    // ===========================================================

    private void SpawnDawn()
    {
        var dawnData = GameManager.Instance.GetSelectedDawn();
        if (dawnData == null || dawnData.Prefab == null)
        {
            Debug.LogError("DawnData 또는 Prefab이 비어있습니다.");
            return;
        }

        // Dawn 생성 및 초기화
        GameObject dawnObj = Instantiate(dawnData.Prefab, dawnSpawnPoint.position, Quaternion.identity);
        var dawn = dawnObj.GetComponent<Dawn>();
        if (dawn != null)
        {
            dawn.Initialize(dawnData);
            spawnedDawn = dawn;
            
            // RadialProgress에 Dawn 연결
            var radialUI = FindObjectOfType<RadialProgress>();
            if (radialUI != null)
            {
                radialUI.SetTargetDawn(dawn);
            }
        }
        else
        {
            Debug.LogWarning("Dawn 컴포넌트를 찾을 수 없습니다.");
        }
    }

    private void UpdateDawnUI()
    {
        if (spawnedDawn == null) return;

        if (hpSlider != null)
            hpSlider.value = spawnedDawn.DawnData.MaxHP > 0 ? spawnedDawn.CurrentHP / spawnedDawn.DawnData.MaxHP : 0f;

        if (energySlider != null)
            energySlider.value = spawnedDawn.DawnData.MaxEnergy > 0 ? (float)spawnedDawn.currentEnergy / spawnedDawn.DawnData.MaxEnergy : 0f;
    }

    // ===========================================================
    // Cost 관련
    // ===========================================================

    private void TryIncreaseCost()
    {
        if (cost < 99)
        {
            cost++;
            UpdateCostText();
        }
    }

    private void UpdateCostText()
    {
        if (costText != null)
            costText.text = cost.ToString();
    }

    public int GetCost() => cost;

    public void DecreaseCost(int amount)
    {
        cost = Mathf.Max(0, cost - amount);
        UpdateCostText();
    }

    // ===========================================================
    // Dark 관련
    // ===========================================================

    public void OnDarkKilled()
    {
        stageDarksCount--;

        if (stageDarksCount <= 0)
        {
            Debug.Log("스테이지 클리어!");
            currentStageState = StageState.Cleared;
            OnStageClear();
        }
    }

    // ===========================================================
    // 스테이지 클리어 처리
    // ===========================================================

    private void OnStageClear()
    {
        // 강화카드가 선택 중이면 클리어 처리 금지
        if (spawnedEnhancementCards.Count > 0)
        {
            Debug.LogWarning("강화 카드 선택 중에 스테이지 클리어 호출됨 - 무시");
            return;
        }
        
        if (stageClearProcessed) return; // 이미 클리어 처리했으면 리턴
        stageClearProcessed = true;
        
        // 카드 스폰 정지
        cardSpawner.canSpawnCards = false;

        // 덱 카드 삭제
        ClearDeckCards();

        // 모든 Ally 제거
        foreach (var allyObj in AllyPoolManager.Instance.activateAllies)
        {
            if (allyObj != null)
            {
                var ally = allyObj.GetComponent<Ally>();
                if (ally != null)
                    ally.ForceDie();
            }
        }

        StartCoroutine(HandleStageClearSequence(stageList[currentStageIndex].StageType));
    }

    private IEnumerator HandleStageClearSequence(StageType stageType)
    {
        if (clearMedalAnimator != null)
        {
            clearMedalAnimator.SetTrigger("isCleared");
            yield return new WaitUntil(() => clearMedalAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClearMedalDown"));
        }

        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.text = "Stage Clear!";
        }

        yield return new WaitForSeconds(clearMedalAndTextTerm);

        stageClearText.gameObject.SetActive(false);

        if (stageType == StageType.Normal)
        {
            ShowEnhancementCardChoices();    
        }
        else if (stageType == StageType.Boss)
        {
            SceneManager.LoadScene("MainScene");
        }
        
    }

    private void ShowEnhancementCardChoices()
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

    public void HideEnhancementCardsAndProceed()
    {
        foreach (var card in spawnedEnhancementCards)
            Destroy(card);

        spawnedEnhancementCards.Clear();
        
        currentStageIndex++; // 스테이지 인덱스 미리 증가
        StartCoroutine(ProceedToNextStageAfterDelay());
    }

    private IEnumerator ProceedToNextStageAfterDelay()
    {
        clearMedalAnimator.SetTrigger("isPlaying");
        yield return new WaitForSeconds(3f);

        if (currentStageIndex >= stageList.Count)
        {
            Debug.Log("모든 스테이지 완료! 게임 종료");
            // 게임 종료 처리
            yield break;
        }

        StartStage();
    }

    // ===========================================================
    // 스테이지 시작
    // ===========================================================

    public bool IsStagePlaying()
    {
        return currentStageState == StageState.Playing;
    }
    
    public void StartStage()
    {
        // 여기서 쿨다운 초기화
        if (spawnedDawn != null)
            spawnedDawn.ResetActiveCooldown();
        
        currentStageState = StageState.Playing;
        stageClearProcessed = false;
        cost = 0;
        costTimer = 0f;
        UpdateCostText();

        cardSpawner.canSpawnCards = true;

        StageData stage = stageList[currentStageIndex];
        stageDarksCount = stage.DarksCount; // 매 스테이지마다 초기화
        stageText.text = stage.StageName;

        darkSpawner.StopSpawning(); // 이전 스폰 루틴 중지
        darkSpawner.StartSpawning(stage);
    }

    private void ClearDeckCards()
    {
        foreach (var slot in cardSlots)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }
    }

    // ===========================================================
    // 외부 호출용
    // ===========================================================

    public void ShiftCardsLeft(int slotIndex)
    {
        for (int i = slotIndex + 1; i < cardSlots.Length; i++)
        {
            if (cardSlots[i].childCount > 0)
            {
                Transform card = cardSlots[i].GetChild(0);
                card.SetParent(cardSlots[i - 1], false);
                
                // 슬롯 인덱스 갱신
                Card cardScript = card.GetComponent<Card>();
                cardScript.slotIndex = i - 1;
            }
            else break;
        }
    }
    
    public void MultiplyCostUp(float multiplier)
    {
        costUpMultiplier *= multiplier;
    }

    public void MultiplyCardSpawnSpeed(float multiplier)
    {
        cardSpawner.spawnInterval /= multiplier;
    }

    public UnitData GetUnitDataByAllyType(AllyType type)
    {
        if (allyUnitDataDict.TryGetValue(type, out var unitData))
            return unitData;

        Debug.LogError($"AllyType {type}에 해당하는 UnitData를 찾을 수 없습니다.");
        return null;
    }
}
