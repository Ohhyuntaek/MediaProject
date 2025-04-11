using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform[] cardSlots;
    
    [SerializeField]
    private List<StageData> stageList;
    [SerializeField]
    private DarkSpawner darkSpawner;

    private int currentStageIndex = 0;
    
    void Awake() => Instance = this;

    void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
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
