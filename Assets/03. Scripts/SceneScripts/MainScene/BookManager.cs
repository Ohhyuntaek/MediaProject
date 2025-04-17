using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class BookManager : MonoBehaviour
{
    public CanvasGroup firstLeftPageButtonGroup;
    public float fadeDuration = 0.5f;
    public float delayAfterBook = 0.1f; // 책 펼친 후 약간 텀

    public Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        // 초기 상태: 숨기고 인터랙션 불가
        firstLeftPageButtonGroup.alpha = 0f;
        firstLeftPageButtonGroup.interactable = false;
        firstLeftPageButtonGroup.blocksRaycasts = false;
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
            firstLeftPageButtonGroup.alpha = alpha;
            yield return null;
        }

        // Fade-in 완료 후 상호작용 가능
        firstLeftPageButtonGroup.interactable = true;
        firstLeftPageButtonGroup.blocksRaycasts = true;
    }

    public void OnNewGameButtonClick()
    {
        firstLeftPageButtonGroup.alpha = 0f;
        firstLeftPageButtonGroup.interactable = false;
        firstLeftPageButtonGroup.blocksRaycasts = false;
        
        animator.SetTrigger("isFlipped");
    }
}