using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnhancementCard : MonoBehaviour
{
    [SerializeField] private TMP_Text enhancementNameText;
    [SerializeField] private TMP_Text enhancementDescriptionText;
    [SerializeField] private Button cardButton;
    [SerializeField] private float darkenAmount = 0.6f;
    
    private EnhancementCardData data;
    private float chosenMultiplier;
    private EnhancementCardData cardData;
    private Dictionary<Image, Color> originalColors = new();

    void Awake()
    {
        foreach (var img in GetComponentsInChildren<Image>())
        {
            if (!originalColors.ContainsKey(img))
                originalColors[img] = img.color;
        }

        cardButton.onClick.AddListener(OnCardSelected);
    }
    
    public void Setup(EnhancementCardData cardData)
    {
        data = cardData;
        chosenMultiplier = Random.Range(data.MinMultiplier, data.MaxMultiplier);
        enhancementNameText.text = data.EnhancementCardName;
        enhancementDescriptionText.text = $"{data.Description}\n<size=80%><color=#888>배수: {chosenMultiplier:F2}x</color>";
    }

    public void OnCardSelected()
    {
        // 클릭 연출
        StartCoroutine(ClickFlashEffect());
        
        switch (data.EnhancementType)
        {
            case EnhancementType.CostUp:
                InStageManager.Instance.MultiplyCostUp(chosenMultiplier);
                break;
            case EnhancementType.SpawnSpeedUp:
                InStageManager.Instance.MultiplyCardSpawnSpeed(chosenMultiplier);
                break;
        }

        InStageManager.Instance.HideEnhancementCardsAndProceed();
    }
    
    private IEnumerator ClickFlashEffect()
    {
        DarkenImages();
        yield return new WaitForSeconds(0.1f);
        RestoreImages();
        yield return new WaitForSeconds(1f);
    }

    private void DarkenImages()
    {
        foreach (var pair in originalColors)
        {
            Color c = pair.Value * darkenAmount;
            c.a = pair.Value.a;
            pair.Key.color = c;
        }
    }

    private void RestoreImages()
    {
        foreach (var pair in originalColors)
        {
            pair.Key.color = pair.Value;
        }
    }
}