using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    FrontLine,
    MidLine,
    RearLine
}

public class Card : MonoBehaviour
{
    public int slotIndex;
    public CardType cardType;
    public AllyType allyType;
    public LineType lineType;
    
    [SerializeField] private Button cardButton; // 카드 클릭용 버튼
    [SerializeField] private TMP_Text costText; // 카드에 표시할 cost 텍스트

    private UnitData unitData;
    private Dictionary<Image, Color> originalColors = new(); // 자식 이미지와 원래 색 저장

    [Header("어둡게 할 계수 (0~1 사이)")]
    [SerializeField] private float darkenMultiplier = 0.6f; // 어둡게 만드는 강도 (예: 60%)
    
    void Start()
    {
        // 생성 시 unitData 가져오기
        unitData = InStageManager.Instance.GetUnitDataByAllyType(allyType);

        if (unitData == null)
        {
            Debug.LogError($"Card에 대한 UnitData를 찾을 수 없습니다. AllyType: {allyType}");
            return;
        }

        // 텍스트에 cost 표시
        if (costText != null)
        {
            costText.text = unitData.Cost.ToString();
        }
        
        // 자식 이미지들의 원본 색 저장
        Image[] childImages = GetComponentsInChildren<Image>();
        foreach (var img in childImages)
        {
            if (!originalColors.ContainsKey(img))
                originalColors.Add(img, img.color);
        }
    }
    
    void Update()
    {
        if (unitData == null) return;

        bool shouldEnable = InStageManager.Instance.GetCost() >= unitData.Cost;

        if (cardButton.interactable != shouldEnable)
        {
            cardButton.interactable = shouldEnable;

            if (shouldEnable)
                RestoreImages();
            else
                DarkenImages();
        }
    }

    private void DarkenImages()
    {
        foreach (var pair in originalColors)
        {
            if (pair.Key != null)
            {
                Color darkColor = pair.Value * darkenMultiplier;
                darkColor.a = pair.Value.a; // 알파는 원래대로 유지
                pair.Key.color = darkColor;
            }
        }
    }

    private void RestoreImages()
    {
        foreach (var pair in originalColors)
        {
            if (pair.Key != null)
            {
                pair.Key.color = pair.Value;
            }
        }
    }
    
    public void OnButtonClick()
    {
        // UnitData를 allyType으로 가져오기
        UnitData unitData = InStageManager.Instance.GetUnitDataByAllyType(allyType);

        if (unitData == null)
        {
            Debug.LogError($"Card에 대한 UnitData를 찾을 수 없습니다. AllyType: {allyType}");
            return;
        }

        if (InStageManager.Instance.GetCost() < unitData.Cost)
        {
            Debug.Log("코스트가 부족합니다! 소환할 수 없습니다.");
            return;
        }
        
        GameObject ally = AllyPoolManager.Instance.SpawnAlly(allyType, lineType);

        if (ally != null)
        {
            Destroy(this.gameObject);
            InStageManager.Instance.ShiftCardsLeft(slotIndex);   
        }
        else
        {
            Debug.Log("No available tile or ally in pool");
        }
    }
}
