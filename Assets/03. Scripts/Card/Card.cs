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
    
    [SerializeField] private Button cardButton; // 카드 클릭용 버튼
    [SerializeField] private TMP_Text costText; // 카드에 표시할 cost 텍스트
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private Image unitImage; 

    private AllyType allyType;
    private LineType lineType;
    private UnitData unitData;
    private Dictionary<Image, Color> originalColors = new(); // 자식 이미지와 원래 색 저장

    [Header("어둡게 할 계수 (0~1 사이)")]
    [SerializeField] private float darkenMultiplier = 0.6f; // 어둡게 만드는 강도 (예: 60%)
    
    void Start()
    {
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

        bool shouldEnable = InGameSceneManager.Instance.costManager.TotalCost >= unitData.Cost;

        if (cardButton.interactable != shouldEnable)
        {
            cardButton.interactable = shouldEnable;

            if (shouldEnable)
                RestoreImages();
            else
                DarkenImages();
        }
    }
    
    public void Setup(UnitData data, int slotIndex, LineType lineType)
    {
        this.unitData = data;
        this.slotIndex = slotIndex;
        this.lineType = lineType;
        this.allyType = data.AllyType;
        this.cardType = (CardType)data.UnitType; // enum 간 매핑

        // cost 텍스트 표시
        if (costText != null)
            costText.text = unitData.Cost.ToString();
        
        // 이미지 설정
        if (unitImage != null && unitData.Sprite != null)
        {
            unitImage.sprite = unitData.Sprite;
        }
    }

    public void OnButtonClick()
    {
        SoundManager.Instance.PlaySfx(selectSound, transform.position, false);
        
        if (unitData == null)
        {
            Debug.LogError($"Card에 대한 UnitData를 찾을 수 없습니다. AllyType: {allyType}");
            return;
        }

        if (InGameSceneManager.Instance.costManager.TotalCost < unitData.Cost)
        {
            Debug.Log("코스트가 부족합니다! 소환할 수 없습니다.");
            return;
        }
        
        GameObject ally = InGameSceneManager.Instance.allyPoolManager.SpawnAlly(unitData, lineType);

        if (ally != null)
        {
            Destroy(this.gameObject);
            InGameSceneManager.Instance.cardSpawner.ShiftCardsLeft(slotIndex);   
        }
        else
        {
            Debug.Log("No available tile or ally in pool");
        }
    }
    
    /// <summary>
    /// 이미지를 어둡게 만드는 함수
    /// </summary>
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

    /// <summary>
    /// 이미지를 원래의 색으로 만드는 함수
    /// </summary>
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

}
