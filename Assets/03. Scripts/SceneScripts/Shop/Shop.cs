using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private CanvasGroup mapCanvasGroup;

    [SerializeField] private float delayAfterMapOpened = 1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float elementDelay = 0.1f;
    [SerializeField] private float slideOffsetX = -100f; // �������� �󸶳� Ƣ�����

    [Header("상점 UI")]
    [SerializeField] private List<Button> buyButtons;
    [SerializeField] private TMP_Text lumenText;
    
    [Header("아이템 SO")] 
    [SerializeField, LabelText("유닛 추가 아이템")] private List<ItemData> addAllyItems;
    [SerializeField, LabelText("유닛 강화 아이템")] private List<ItemData> remnantItems;
    
    [Header("아이템 UI")]
    [SerializeField] private List<Image> itemImages;
    [SerializeField] private List<TMP_Text> itemPrices;
    [SerializeField] private List<TMP_Text> itemDescriptions;
    
    private List<ItemData> chosenItems = new();
    
    void Start()
    {
        mapAnimator = GetComponent<Animator>();
        SetCanvasGroupActive(mapCanvasGroup, false);
        SetupShopItems();
    }

    void SetupShopItems()
    {
        // 1. 중복 없이 3개씩 랜덤 선택
        List<ItemData> randomAllies = addAllyItems.OrderBy(x => Random.value).Take(3).ToList();
        List<ItemData> randomRemnants = remnantItems.OrderBy(x => Random.value).Take(3).ToList();
        chosenItems = randomAllies.Concat(randomRemnants).ToList(); // 총 6개

        for (int i = 0; i < buyButtons.Count && i < chosenItems.Count; i++)
        {
            int index = i; // 로컬 변수 캡처 주의 (Closure 이슈 방지)
            ItemData data = chosenItems[i];

            // 자식 Image 및 Text 가져오기
            Image image = itemImages[i].GetComponentInChildren<Image>();
            TMP_Text priceText = itemPrices[i].GetComponentInChildren<TMP_Text>();
            TMP_Text descriptionText = itemDescriptions[i].GetComponentInChildren<TMP_Text>();
            
            if (image != null) image.sprite = data.ItemImage;
            if (priceText != null) priceText.text = $"{data.ItemPrice}";
            if (descriptionText != null) descriptionText.text = data.Description;

            // 버튼 클릭 이벤트 할당
            buyButtons[i].onClick.RemoveAllListeners();
            buyButtons[i].onClick.AddListener(() =>
            {
                int currentLumen = RuntimeDataManager.Instance.lumenCalculator.Lumen;

                if (currentLumen < data.ItemPrice)
                {
                    Debug.Log($"구매 불가: 현재 Lumen({currentLumen})이 {data.ItemPrice}보다 부족합니다.");
                    return;
                }

                // 구매 성공 시:
                RuntimeDataManager.Instance.lumenCalculator.RemoveLumen(data.ItemPrice);

                RuntimeDataManager.Instance.itemCollector.SelectItem(data);
                
                SceneManager.LoadScene("MapScene");
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
            // CanvasGroup ������ �߰�
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null) cg = child.gameObject.AddComponent<CanvasGroup>();

            // �ʱ� ����
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // ��ġ ���� (�������� �̵�)
            RectTransform rect = child.GetComponent<RectTransform>();
            Vector3 originalPos = rect.anchoredPosition;
            rect.anchoredPosition = originalPos + new Vector3(slideOffsetX, 0f, 0f);

            // Ʈ�� ����
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
