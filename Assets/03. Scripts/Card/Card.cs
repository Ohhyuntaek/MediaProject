using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum CardType
{
    FrontLine,
    MidLine,
    RearLine
}

public class Card : MonoBehaviour,IPointerEnterHandler,
    IPointerExitHandler
{
    public int slotIndex;
    public CardType cardType;
    
    [SerializeField] private Button cardButton; // 카드 클릭용 버튼
    [SerializeField] private TMP_Text costText; // 카드에 표시할 cost 텍스트
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image costColor;
    [SerializeField] private MMF_Player spawnFeedback;
    [Header("어둡게 할 계수 (0~1 사이)")]
    [SerializeField] private float darkenMultiplier = 0.6f; // 어둡게 만드는 강도 (예: 60%)
    private bool hasHovered = false;
    private AllyType allyType;
    private LineType lineType;
    private UnitData unitData;
    private Dictionary<Image, Color> originalColors = new(); // 자식 이미지와 원래 색 저장
    
    
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

        bool shouldEnable = UIManager.Instance.costManager.TotalCost >= unitData.Cost;

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
        
        switch (lineType)
        {
            case LineType.Front:
            {
                costColor.color = HexToColor("#F75A5A");
                break;
            } 
            case LineType.Mid:
            {
                costColor.color = HexToColor("#FFD63A");
                break;
            }
            case LineType.Rear:
            {
                costColor.color = HexToColor("#6DE1D2");
                break;
            }
            default:
                break;
        }
        
        // cost 텍스트 표시
        if (costText != null)
            costText.text = unitData.Cost.ToString();
        
        // 이미지 설정
        if (unitImage != null && unitData.Sprite != null)
        {
            unitImage.sprite = unitData.Sprite;
        }
        
        if (spawnFeedback != null)
            spawnFeedback.PlayFeedbacks();
    }

    public void OnButtonClick()
    {
        EntireGameManager.Instance.soundManager.PlaySfx(selectSound, transform.position, false);
        
        if (unitData == null)
        {
            Debug.LogError($"Card에 대한 UnitData를 찾을 수 없습니다. AllyType: {allyType}");
            return;
        }

        if (UIManager.Instance.costManager.TotalCost < unitData.Cost)
        {
            Debug.Log("코스트가 부족합니다! 소환할 수 없습니다.");
            return;
        }
        
        if (!InGameSceneManager.Instance.tileManager.PreviewAvailableTile())
        {
            Debug.Log("No available tile or ally in pool");
            return;
        }
        
        // 이펙트 제거 후 스폰
        InGameSceneManager.Instance.tileManager.ClearSpawnTileEffect();
        GameObject ally = InGameSceneManager.Instance.allyPoolManager.SpawnAlly(unitData, lineType);

        if (ally != null)
        {
            Destroy(this.gameObject);
            InGameSceneManager.Instance.cardSpawner.ShiftCardsLeft(slotIndex);   
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

    public void OnPointerEnter(PointerEventData eventData)
    {
       
        InGameSceneManager.Instance.previewManager.ShowPreview(unitData);
        InGameSceneManager.Instance.previewManager.ShowTooltip(unitData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InGameSceneManager.Instance.previewManager.ClearPreview();
        InGameSceneManager.Instance.previewManager.ClearToolTip();
    }
    
    /// <summary>
    /// Hex를 Color로 리턴합니다.
    /// </summary>
    /// <param name="hex">Hex (#생략 가능)</param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", ""); // "#" 문자 제거
        if (hex.Length != 6)
        {
            Debug.LogError("유효하지 않은 Hex 값입니다.");
            return Color.white;
        }

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }
}
