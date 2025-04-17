using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BookUISequence : MonoBehaviour
{
    public CanvasGroup buttonGroup;
    public float fadeDuration = 0.5f;
    public float delayAfterBook = 0.1f; // 책 펼친 후 약간 텀

    void Start()
    {
        // 초기 상태: 숨기고 인터랙션 불가
        buttonGroup.alpha = 0f;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;

        StartCoroutine(FadeInButtons());
    }

    IEnumerator FadeInButtons()
    {
        // 책 펼쳐진 뒤 약간 기다림 (필요 시 애니메이션 시간에 맞춰 조절)
        yield return new WaitForSeconds(delayAfterBook);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            buttonGroup.alpha = alpha;
            yield return null;
        }

        // Fade-in 완료 후 상호작용 가능
        buttonGroup.interactable = true;
        buttonGroup.blocksRaycasts = true;
    }
}