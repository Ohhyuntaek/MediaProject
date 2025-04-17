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
    }

    public void OnNewGameButtonClick()
    {
        SetCanvasGroupActive(titlePageGroup, false);
        animator.SetTrigger("isFlipped");
    }

    public IEnumerator FadeInButtons()
    {
        yield return new WaitForSeconds(delayAfterBook);
        yield return StartCoroutine(FadeCanvasGroup(titlePageGroup, fadeDuration, true));
    }

    void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }

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