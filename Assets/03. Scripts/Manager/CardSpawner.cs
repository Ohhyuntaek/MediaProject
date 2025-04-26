using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public Card[] defenderCards;
    public Transform[] cardSlots;
    public float spawnInterval = 2f;

    [SerializeField] private int cardSlotNumber = 8;
    private float timer = 0f;
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval)
        {
            TrySpawnCard();
            timer = 0f;
        }
    }

    void TrySpawnCard()
    {
        // 덱이 가득 찼는지 확인
        if (cardSlots[cardSlotNumber-1].childCount > 0) return;

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
        List<Card> availableCards = new List<Card>();
        foreach (Card cardPrefab in defenderCards)
        {
            if (!typeCounts.ContainsKey(cardPrefab.cardType) || typeCounts[cardPrefab.cardType] < 3)
            {
                availableCards.Add(cardPrefab);
            }
        }

        // 생성 가능한 카드가 없으면 생성하지 않음
        if (availableCards.Count == 0) return;

        // 비어있는 가장 왼쪽 슬롯 찾기
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i].childCount == 0)
            {
                // 가능한 카드 중 랜덤 선택
                Card card = Instantiate(availableCards[Random.Range(0, availableCards.Count)]);
                card.transform.SetParent(cardSlots[i], false);
                card.slotIndex = i;
                break;
            }
        }
    }
}
