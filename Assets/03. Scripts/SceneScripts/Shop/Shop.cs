using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private CanvasGroup mapCanvasGroup;

    [SerializeField] private float delayAfterMapOpened = 1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float elementDelay = 0.1f;
    [SerializeField] private float slideOffsetX = -100f;

    [Header("상점 UI")]
    [SerializeField] private List<Button> buyButtons;
    [SerializeField] private TMP_Text lumenText;

    [Header("모든 아이템 SO")]
    [SerializeField, LabelText("아이템 리스트 (AllyItemData, RemnantSO 등)")]
    private List<ItemEffectBase> allItems;

    [Header("아이템 UI")]
    [SerializeField] private List<Image> itemImages;
    [SerializeField] private List<TMP_Text> itemPrices;
    [SerializeField] private List<TMP_Text> itemDescriptions;

    private List<ItemEffectBase> chosenItems = new();

    void Start()
    {
        
        mapAnimator = GetComponent<Animator>();
        SetCanvasGroupActive(mapCanvasGroup, false);

        // 초기 Lumen 표시
        lumenText.text = RuntimeDataManager.Instance.lumenCalculator.Lumen.ToString();

        SetupShopItems();
    }

    void SetupShopItems()
    {
        // 리스트에서 중복 없이 랜덤으로 6개 선택
        chosenItems = allItems
            .OrderBy(x => Random.value)
            .Take(6)
            .ToList();

        for (int i = 0; i < buyButtons.Count && i < chosenItems.Count; i++)
        {
            var data = chosenItems[i];

            // UI 세팅
            if (i < itemImages.Count && itemImages[i] != null)
                itemImages[i].sprite = data.Icon;

            if (i < itemPrices.Count && itemPrices[i] != null)
                itemPrices[i].text = data.Price.ToString();

            if (i < itemDescriptions.Count && itemDescriptions[i] != null)
                itemDescriptions[i].text = data.Description;

            // 버튼 클릭 이벤트
            buyButtons[i].onClick.RemoveAllListeners();
            buyButtons[i].onClick.AddListener(() =>
            {
                int currentLumen = RuntimeDataManager.Instance.lumenCalculator.Lumen;
                if (currentLumen < data.Price)
                {
                    Debug.Log($"구매 불가: 현재 Lumen({currentLumen})이 {data.Price}보다 부족합니다.");
                    return;
                }

                // 구매 처리
                Debug.Log("구매 완료" + data.Description);
                RuntimeDataManager.Instance.lumenCalculator.RemoveLumen(data.Price);
                lumenText.text = RuntimeDataManager.Instance.lumenCalculator.Lumen.ToString();

                RuntimeDataManager.Instance.itemCollector.SelectItem(data);
            });
        }
    }

    public void FadeInMap()
    {
        SetCanvasGroupActive(mapCanvasGroup, true);
        mapCanvasGroup.alpha = 1f;

        Transform parent = mapCanvasGroup.transform;
        int index = 0;

        foreach (Transform child in parent)
        {
            CanvasGroup cg = child.GetComponent<CanvasGroup>() ?? child.gameObject.AddComponent<CanvasGroup>();
            
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            RectTransform rect = child.GetComponent<RectTransform>();
            Vector3 originalPos = rect.anchoredPosition;
            rect.anchoredPosition = originalPos + new Vector3(slideOffsetX, 0f, 0f);

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delayAfterMapOpened + index * elementDelay);
            seq.Append(rect.DOAnchorPos(originalPos, fadeDuration).SetEase(Ease.OutCubic));
            seq.Join(cg.DOFade(1f, fadeDuration));
            seq.OnComplete(() =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });

            index++;
        }
    }

    void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
}
