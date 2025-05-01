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
    [SerializeField] private AudioClip selectSound;

    private EnhancementCardData data;
    private float defferenceRandomValue;
    private Dictionary<Image, Color> originalColors = new();

    private bool isClicked = false; // 클릭 중복 방지용
    private Dawn playerDawn;
    
    void Awake()
    {
        foreach (var img in GetComponentsInChildren<Image>())
        {
            if (!originalColors.ContainsKey(img))
                originalColors[img] = img.color;
        }

        cardButton.onClick.AddListener(OnCardSelected);
        
        playerDawn = FindObjectOfType<Dawn>(); // Dawn 객체 참조 저장
    }

    public void Setup(EnhancementCardData cardData)
    {
        data = cardData;
        defferenceRandomValue = Random.Range(data.MinRangeValue, data.MaxRangeValue);
        enhancementNameText.text = data.EnhancementCardName;
        enhancementDescriptionText.text = $"{data.Description}\n<size=100%><color=#888>배수: {defferenceRandomValue:F2}x</color>";
    }

    public void OnCardSelected()
    {
        if (isClicked) return; // 이미 클릭했으면 무시
        isClicked = true;
        
        // 카드 선택 시 소리 재생
        SoundManager.Instance.PlaySfx(selectSound, transform.position, false);

        StartCoroutine(ClickFlashEffectAndProceed());
    }

    private IEnumerator ClickFlashEffectAndProceed()
    {
        DarkenImages();

        // 강화 효과 적용
        switch (data.EnhancementType)
        {
            case EnhancementType.CostUp:
                InGameSceneManager.Instance.costManager.CostSpeedUp(defferenceRandomValue);
                break;
            case EnhancementType.CardSpawnSpeedUp:
                InGameSceneManager.Instance.cardSpawner.CardSpawnSpeedUP(defferenceRandomValue);
                break;
            case EnhancementType.CooldownSpeedUp:
                if (playerDawn != null)
                    playerDawn.CooldownMultiplier *= defferenceRandomValue;
                break;
            case EnhancementType.EnergyChargeSpeedUp:
                if (playerDawn != null)
                    playerDawn.EnergyChargeMultiplier *= defferenceRandomValue;
                break;
        }

        // 클릭 후 0.1초 기다렸다 복구
        yield return new WaitForSeconds(0.1f);
        RestoreImages();

        // 추가로 1초 정도 연출 시간 주기
        yield return new WaitForSeconds(0.3f);

        // 강화 카드 숨기고 다음 스테이지로 넘어가기
        InGameSceneManager.Instance.clearUIManager.HideEnhancementCardsAndProceed();
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
