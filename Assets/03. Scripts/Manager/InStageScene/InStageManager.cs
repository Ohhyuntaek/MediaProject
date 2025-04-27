using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class AllyUnitDataLink
{
    // AllyType과 UnitData를 연결
    public AllyType allyType;
    public UnitData unitData;
}

public enum StageState
{
    Playing,
    Cleared
}

public class InStageManager : MonoBehaviour
{
    public static InStageManager Instance;
    
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private List<StageData> stageList;
    [SerializeField] private List<Sprite> dawnImages;
    [SerializeField] private List<AllyUnitDataLink> allyUnitDataLinks;
    [SerializeField] private TMPro.TMP_Text stageText;
    [SerializeField] private TMPro.TMP_Text stageClearText;
    [SerializeField] private DarkSpawner darkSpawner;
    [FormerlySerializedAs("playerImage")] [SerializeField] private Image dawnImage;
    [SerializeField] private TMPro.TMP_Text costText;
    [SerializeField] private CardSpawner cardSpawner;
    [SerializeField] private float costUpMultiplier = 1.0f; // 코스트 상승 속도 (외부 조정 가능)
    [SerializeField] private Transform dawnSpawnPoint; // Dawn이 스폰될 위치
    [SerializeField] private Slider hpSlider;        // Dawn 체력 표시용
    [SerializeField] private Slider energySlider;    // Dawn 에너지 표시용
    
    private Dictionary<AllyType, UnitData> allyUnitDataDict = new();
    private int cost = 0;          // 현재 코스트
    private float costTimer = 0f;  // 코스트 상승 타이머
    private float costUpInterval => 1.0f / costUpMultiplier; // 실제 코스트 증가 주기 (초)
    private GameObject player;
    private int currentStageIndex = 0;
    private int aliveDarkCount = 0;
    private Dawn spawnedDawn; // 소환된 Dawn을 저장할 변수
    private StageState currentStageState = StageState.Playing;
    
    private void Awake()
    {
        Instance = this;
        
        // AllyType으로 UnitData의 cost를 받아오기 위해 작성한 코드
        foreach (var link in allyUnitDataLinks)
        {
            if (!allyUnitDataDict.ContainsKey(link.allyType))
            {
                allyUnitDataDict.Add(link.allyType, link.unitData);
            }
            else
            {
                Debug.LogWarning($"Duplicate AllyType: {link.allyType} in allyUnitDataLinks!");
            }
        }
    }
    /// <summary>
    /// AllyType에 맞는 UnitData를 get함
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public UnitData GetUnitDataByAllyType(AllyType type)
    {
        if (allyUnitDataDict.TryGetValue(type, out var unitData))
        {
            return unitData;
        }
    
        Debug.LogError($"AllyType {type}에 해당하는 UnitData를 찾을 수 없습니다.");
        return null;
    }

    void Start()
    {
        // GameManager에서 선택한 Dawn 정보를 가져옴
        DawnData selectedDawn = GameManager.Instance.GetSelectedDawn();

        if (selectedDawn == null)
        {
            Debug.LogError("GameManager에 저장된 DawnData가 없습니다!");
            return;
        }

        // Dawn 이미지 직접 세팅
        if (dawnImage != null)
        {
            dawnImage.sprite = selectedDawn.Portrait;
        }
    
        StartStage();
        // Dawn 소환
        SpawnDawn();
    }
    
    void Update()
    {
        if (currentStageState != StageState.Playing)
            return; // 클리어 상태면 코스트 증가/슬라이더 업데이트 금지
        
        // 매 프레임 cost 타이머 누적
        costTimer += Time.deltaTime;

        if (costTimer >= costUpInterval)
        {
            TryIncreaseCost();
            costTimer = 0f; // 타이머 리셋
        }
        
        // Dawn 체력/에너지 UI 업데이트
        UpdateDawnUI();
    }
    
    private void UpdateDawnUI()
    {
        if (spawnedDawn == null) return;

        if (hpSlider != null)
            hpSlider.value = spawnedDawn.DawnData.MaxHP > 0 ? spawnedDawn.CurrentHP / spawnedDawn.DawnData.MaxHP : 0f;
    
        if (energySlider != null)
            energySlider.value = spawnedDawn.DawnData.MaxEnergy > 0 ? (float)spawnedDawn.Energy / spawnedDawn.DawnData.MaxEnergy : 0f;
    }
    
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

    public int GetCost()
    {
        return cost;
    }
    
    public void DecreaseCost(int amount)
    {
        cost = Mathf.Max(0, cost - amount); // cost는 0 미만으로 내려가지 않음
        UpdateCostText();
    }
    
    /// <summary>
    /// Dark 하나 스폰될 때마다 호출
    /// </summary>
    public void OnDarkSpawned()
    {
        aliveDarkCount++;
    }
    
    /// <summary>
    /// Dark 하나 죽을 때마다 호출
    /// </summary>
    public void OnDarkKilled()
    {
        aliveDarkCount--;

        if (aliveDarkCount <= 0)
        {
            Debug.Log("스테이지 클리어!");
            OnStageClear();
        }
    }

    private void OnStageClear()
    {
        currentStageState = StageState.Cleared; // 상태를 Cleared로 변경
        
        // 카드 스폰 정지
        cardSpawner.canSpawnCards = false;
    
        // 덱에 남은 카드 삭제
        ClearDeckCards();

        // 1. 현재 활성화된 모든 Ally들의 duration을 0으로
        foreach (var allyObj in AllyPoolManager.Instance.activateAllies)
        {
            if (allyObj != null)
            {
                Ally ally = allyObj.GetComponent<Ally>();
                if (ally != null)
                {
                    ally.ForceDie(); // 부활 없이 즉시 사망
                }
            }
        }

        // 2. Stage Clear 문구 띄우기
        StartCoroutine(HandleStageClearSequence(stageList[currentStageIndex].StageType));
    }
    
    private IEnumerator HandleStageClearSequence(StageType stageType)
    {
        // Stage Clear 텍스트 활성화
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.text = "Stage Clear!";
        }

        // 2초 동안 대기
        yield return new WaitForSeconds(5f);

        // Stage Clear 텍스트 비활성화
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(false);
        }

        // 다음 스테이지로 이동
        if (stageType == StageType.Boss)
        {
            Debug.Log("보스 스테이지 클리어! 게임 종료!");
            // 게임 종료 처리 추가
        }
        else
        {
            Debug.Log("다음 스테이지로 이동!");
            currentStageIndex++;

            if (currentStageIndex < stageList.Count)
            {
                StartStage();
            }
            else
            {
                Debug.Log("모든 스테이지 완료! 게임 끝!");
                // 게임 완료 처리 추가
            }
        }
    }
    
    private void ClearDeckCards()
    {
        foreach (var slot in cardSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }

    public void StartStage()
    {
        currentStageState = StageState.Playing; // 다시 Playing 상태로 전환
        
        cardSpawner.canSpawnCards = true; // 카드 스폰 재허용
        
        // 코스트 초기화
        cost = 0;
        UpdateCostText(); // 코스트 텍스트도 바로 갱신
        
        stageText.text = stageList[currentStageIndex].StageName;
        StageData stage = stageList[currentStageIndex];
        darkSpawner.StartSpawning(stage);
    }
    
    private void SpawnDawn()
    {
        DawnData dawnData = GameManager.Instance.GetSelectedDawn();

        if (dawnData == null)
        {
            Debug.LogError("선택된 DawnData가 없습니다!");
            return;
        }

        if (dawnData.Prefab == null)
        {
            Debug.LogError("DawnData에 연결된 Prefab이 없습니다!");
            return;
        }

        // Instantiate DawnPrefab at Spawn Point
        GameObject dawnObj = Instantiate(dawnData.Prefab, dawnSpawnPoint.position, Quaternion.identity);

        // Initialize Dawn 컴포넌트
        Dawn dawn = dawnObj.GetComponent<Dawn>();
        if (dawn != null)
        {
            dawn.Initialize(dawnData);
            spawnedDawn = dawn; // 소환된 Dawn 저장
        }
        else
        {
            Debug.LogWarning("Dawn 컴포넌트를 찾을 수 없습니다.");
        }
    }
    
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
}
