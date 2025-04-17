using System.Collections;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public CanvasGroup titlePageGroup;
    public CanvasGroup newGamePageGroup;
    public float fadeDuration = 0.5f;
    public float delayAfterBook = 0.1f;

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetCanvasGroupActive(titlePageGroup, false);
        SetCanvasGroupActive(newGamePageGroup, false);
    }

    /// new game 버튼 클릭 시
    public void OnNewGameButtonClick()
    {
        SetCanvasGroupActive(titlePageGroup, false);
        SetCanvasGroupActive(newGamePageGroup, true);
        // 책 넘기기 애니메이션 진행
        animator.SetTrigger("isFlipped");
    }

    public IEnumerator FadeInButtons()
    {
        // 애니메이션 이벤트로 추가 (수정 예정)
        yield return new WaitForSeconds(delayAfterBook);
        yield return StartCoroutine(FadeCanvasGroup(titlePageGroup, fadeDuration, true));
    }
    
    /// 해당되는 Canvas Group의 상호작용 및 보이기 제거
    void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }

    
    /// Canvas Group의 색이 Fade out으로 점차 사라지게끔 만드는 코루틴 
    IEnumerator FadeCanvasGroup(CanvasGroup group, float duration, bool fadeIn, System.Action onComplete = null)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        float elapsed = 0f;
        group.alpha = startAlpha;
        group.interactable = false;
        group.blocksRaycasts = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            group.alpha = alpha;
            yield return null;
        }

        group.alpha = endAlpha;
        group.interactable = fadeIn;
        group.blocksRaycasts = fadeIn;

        onComplete?.Invoke();
    }
}