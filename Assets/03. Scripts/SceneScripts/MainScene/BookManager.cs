using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour
{
    public CanvasGroup titlePageGroup;
    public CanvasGroup newGamePageGroup;
    public float fadeDuration = 0.5f;
    public float delayAfterBook = 0.1f;
    public AudioClip pageFlipSound;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        SetCanvasGroupActive(titlePageGroup, false);
        SetCanvasGroupActive(newGamePageGroup, false);
    }

    /// <summary>
    /// titlePage - new game 버튼 클릭 시
    /// </summary>
    public void OnNewGameButtonClick()
    {
        SetCanvasGroupActive(titlePageGroup, false);
        
        // 책 넘기기 소리 재생
        PlayPageFlipSound();
        
        // 책 넘기기 애니메이션 진행
        animator.SetTrigger("isFlipped");
    }
    
    /// <summary>
    /// newGamePage - Back버튼 클릭 시
    /// </summary>
    public void OnBackButtonClick()
    {
        SetCanvasGroupActive(newGamePageGroup, false);
        
        // 책 넘기기 소리 재생
        PlayPageFlipSound();
        
        // 책 반대로 넘기기 애니메이션 진행
        animator.SetTrigger("isBackFlipped");
    }

    /// <summary>
    /// newGamePage - Start 버튼 클릭 시
    /// </summary>
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("InStage");
    }
    
    void PlayPageFlipSound()
    {
        if (audioSource != null && pageFlipSound != null)
        {
            audioSource.PlayOneShot(pageFlipSound);
        }
    }

    public IEnumerator FadeInTitleGroup()
    {
        // OpenBook 애니메이션의 이벤트로 추가 - 책이 펼쳐지면 delayAfterBook 시간 후 titlePageGroup이 생성
        yield return new WaitForSeconds(delayAfterBook);
        yield return StartCoroutine(FadeCanvasGroup(titlePageGroup, fadeDuration, true));
    }

    public IEnumerator FadeInNewGameGroup()
    {
        // PageFlip(NewGame) 애니메이션의 이벤트로 추가 - 책이 펼쳐지면 delayAfterBook 시간 후 newGamePageGroup이 생성
        yield return new WaitForSeconds(delayAfterBook);
        yield return StartCoroutine(FadeCanvasGroup(newGamePageGroup, fadeDuration, true));
    }
    
    /// 해당되는 Canvas Group의 상호작용 및 보이기 제거
    void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
    
    /// Canvas Group의 색이 Fade out/in으로 점차 사라지게끔 만드는 코루틴 
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