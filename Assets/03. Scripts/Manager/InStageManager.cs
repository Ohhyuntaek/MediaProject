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

public class InStageManager : MonoBehaviour
{
    public static InStageManager Instance;
    
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private List<StageData> stageList;
    [SerializeField] private List<Sprite> playerImages;
    [SerializeField] private List<AllyUnitDataLink> allyUnitDataLinks;
    [SerializeField] private TMPro.TMP_Text stageText;
    [SerializeField] private TMPro.TMP_Text stageClearText;
    [SerializeField] private DarkSpawner darkSpawner;
    [SerializeField] private Image playerImage;
    [SerializeField] private TMPro.TMP_Text costText;
    [SerializeField] private float costUpMultiplier = 1.0f; // 코스트 상승 속도 (외부 조정 가능)

    private Dictionary<AllyType, UnitData> allyUnitDataDict = new();
    private int cost = 0;                    // 현재 코스트
    private float costTimer = 0f;             // 코스트 상승 타이머
    private float costUpInterval => 1.0f / costUpMultiplier; // 실제 코스트 증가 주기 (초)
    private GameObject player;
    private int currentStageIndex = 0;
    private int aliveDarkCount = 0;
    
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
        // 1. Player 태그를 가진 오브젝트를 찾아 player 변수에 저장
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // 2. player의 _playerData.PlayerName을 가져옴
        Player playerController = player.GetComponent<Player>();
        if (playerController == null)
        {
            Debug.LogError("Player 오브젝트에 Player 컴포넌트가 없습니다.");
            return;
        }

        string playerName = playerController.PlayerData.PlayerName;

        // 3. playerImages 리스트에서 이름이 매칭되는 스프라이트 찾기
        foreach (var sprite in playerImages)
        {
            if (sprite.name.Contains(playerName))
            {
                playerImage.sprite = sprite;
                break;
            }
        }
        
        StartStage();
    }
    
    void Update()
    {
        // 매 프레임 cost 타이머 누적
        costTimer += Time.deltaTime;

        if (costTimer >= costUpInterval)
        {
            TryIncreaseCost();
            costTimer = 0f; // 타이머 리셋
        }
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
        StageData currentStage = stageList[currentStageIndex];

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
        StartCoroutine(HandleStageClearSequence(currentStage.StageType));
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
        yield return new WaitForSeconds(2f);

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

    public void StartStage()
    {
        // 코스트 초기화
        cost = 0;
        UpdateCostText(); // 코스트 텍스트도 바로 갱신
        
        stageText.text = stageList[currentStageIndex].StageName;
        StageData stage = stageList[currentStageIndex];
        darkSpawner.StartSpawning(stage);
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
