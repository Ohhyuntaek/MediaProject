using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private CanvasGroup mapCanvasGroup;

    [SerializeField] private float delayAfterMapOpened = 1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float elementDelay = 0.1f;
    [SerializeField] private float slideOffsetX = -100f; // 왼쪽으로 얼마나 튀어나올지

    void Start()
    {
        mapAnimator = GetComponent<Animator>();
        SetCanvasGroupActive(mapCanvasGroup, false);
    }

    public void FadeInMap()
    {
        SetCanvasGroupActive(mapCanvasGroup, true);
        mapCanvasGroup.alpha = 1f;

        Transform parent = mapCanvasGroup.transform;
        int index = 0;

        foreach (Transform child in parent)
        {
            // CanvasGroup 없으면 추가
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null) cg = child.gameObject.AddComponent<CanvasGroup>();

            // 초기 설정
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // 위치 설정 (왼쪽으로 이동)
            RectTransform rect = child.GetComponent<RectTransform>();
            Vector3 originalPos = rect.anchoredPosition;
            rect.anchoredPosition = originalPos + new Vector3(slideOffsetX, 0f, 0f);

            // 트윈 생성
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
