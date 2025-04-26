using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private List<StageData> stageList;
    [SerializeField] private List<Sprite> playerImages;
    [SerializeField] private TMPro.TMP_Text stageText;
    [SerializeField] private DarkSpawner darkSpawner;
    [SerializeField] private Image playerImage;

    private GameObject player;
    private int currentStageIndex = 0;
    private int aliveDarkCount = 0;
    
    void Awake() => Instance = this;

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
    
    // Dark 하나 스폰될 때마다 호출
    public void OnDarkSpawned()
    {
        aliveDarkCount++;
    }
    
    // Dark 하나 죽을 때마다 호출
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

        if (currentStage.StageType == StageType.Boss)
        {
            Debug.Log("보스 스테이지 클리어! 게임 종료!");
            // 게임 종료 처리 (타이틀 화면 이동 등 추가 가능)
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
                // 추가적인 게임 완료 처리
            }
        }
    }

    public void StartStage()
    {
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
