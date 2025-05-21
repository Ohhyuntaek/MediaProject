using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private CanvasGroup mapCanvasGroup;

    [SerializeField] private float delayAfterMapOpened = 1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float elementDelay = 0.1f;
    [SerializeField] private float slideOffsetX = -100f; // �������� �󸶳� Ƣ�����

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
