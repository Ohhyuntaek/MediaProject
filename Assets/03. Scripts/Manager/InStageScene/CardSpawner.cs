using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CardSpawner : MonoBehaviour
{
    public float cardSpawnIntervalMultiplier = 2f;
    
    [SerializeField] private int cardSlotNumber = 8;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<UnitData> unitDataList;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private bool canSpawnCards = true;
    
    private float cardSpawnTimer = 0f;
    private const float cardSpawnInterval = 1f;

    public bool CanSpawnCards
    {
        get { return canSpawnCards; }
        set { canSpawnCards = value; }
    }
    
    private float totalCardSpawnInterval
    {
        get => cardSpawnInterval * cardSpawnIntervalMultiplier;
        set => totalCardSpawnInterval = value;
    }

    private void Update()
    {
        if (canSpawnCards)
        {
            cardSpawnTimer += Time.deltaTime;
            
            if (cardSpawnTimer >= totalCardSpawnInterval)
            {
                SpawnCard();
                cardSpawnTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// 다음 카드가 스폰하기까지 걸리는 시간을 감소하는 함수
    /// </summary>
    /// <param name="differenceValue">감소할 값</param>
    public void CardSpawnSpeedUP(float differenceValue)
    {
        cardSpawnIntervalMultiplier -= differenceValue;
    }
    
    /// <summary>
    /// 덱에 있는 카드 전부 제거
    /// </summary>
    public void ClearDeckCards()
    {
        foreach (var slot in cardSlots)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }
    }
    
    /// <summary>
    /// 덱에 있는 카드를 왼쪽으로 정렬
    /// </summary>
    /// <param name="slotIndex"></param>
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
    
    /// <summary>
    /// 카드 스폰 (같은 종류의 카드가 3개 이상 스폰되지 않게 함)
    /// </summary>
    private void SpawnCard()
    {
        if (cardSlots[cardSlotNumber - 1].childCount > 0) return;

        // 현재 카드 타입 개수 계산
        Dictionary<CardType, int> typeCounts = new Dictionary<CardType, int>();
        foreach (Transform slot in cardSlots)
        {
            if (slot.childCount > 0)
            {
                Card existingCard = slot.GetChild(0).GetComponent<Card>();
                if (typeCounts.ContainsKey(existingCard.cardType))
                    typeCounts[existingCard.cardType]++;
                else
                    typeCounts[existingCard.cardType] = 1;
            }
        }

        // 3개 미만인 타입만 모음
        List<UnitData> availableData = new List<UnitData>();
        foreach (var data in unitDataList)
        {
            CardType cardType = ConvertUnitTypeToCardType(data.UnitType);
            if (!typeCounts.ContainsKey(cardType) || typeCounts[cardType] < 3)
            {
                availableData.Add(data);
            }
        }

        if (availableData.Count == 0) return;

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i].childCount == 0)
            {
                UnitData selectedData = availableData[Random.Range(0, availableData.Count)];
                GameObject cardObj = Instantiate(cardPrefab, cardSlots[i], false);
                Card card = cardObj.GetComponent<Card>();

                LineType lineType = ConvertUnitTypeToLineType(selectedData.UnitType);
                card.Setup(selectedData, i, lineType);

                break;
            }
        }
    }
    
    /// <summary>
    ///  CardType 변환 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private CardType ConvertUnitTypeToCardType(UnitType type)
    {
        return type switch
        {
            UnitType.Front => CardType.FrontLine,
            UnitType.Middle => CardType.MidLine,
            UnitType.Rear => CardType.RearLine,
            _ => CardType.MidLine
        };
    }
    
    private LineType ConvertUnitTypeToLineType(UnitType type)
    {
        return type switch
        {
            UnitType.Front => LineType.Front,
            UnitType.Middle => LineType.Mid,
            UnitType.Rear => LineType.Rear,
            _ => LineType.Mid
        };
    }

}
