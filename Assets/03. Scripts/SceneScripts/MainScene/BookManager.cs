using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float delayAfterBook = 0.5f;
    
    [SerializeField]
    private AudioClip bookOnWindowSound;
    [SerializeField]
    private AudioClip bookOpenSound;
    [SerializeField]
    private AudioClip pageFlipSound;
    [SerializeField]
    private CanvasGroup titlePageGroup;
    [SerializeField]
    private CanvasGroup newGamePageGroup;
    [SerializeField]
    private CanvasGroup settingPageGroup;
    
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetCanvasGroupActive(titlePageGroup, false);
        SetCanvasGroupActive(newGamePageGroup, false);
    }

    private void Awake()
    {
        // SoundManager.Instance.PlaySfx(bookOnWindowSound,transform.position,false);
    }

    /// <summary>
    /// titlePage - new game 버튼 클릭 시
    /// </summary>
    public void OnNewGameButtonClick()
    {
        SetCanvasGroupActive(titlePageGroup, false);
        SetCanvasGroupActive(settingPageGroup, false);
        
        // 책 넘기기 소리 재생
        SoundManager.Instance.PlaySfx(pageFlipSound,transform.position,false);
        
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
        SoundManager.Instance.PlaySfx(pageFlipSound,transform.position,false);
        
        // 책 반대로 넘기기 애니메이션 진행
        animator.SetTrigger("isBackFlipped");
    }

    bool isSettingOn = false;
    public void OnSettingButtonClick()
    {
        if (!isSettingOn)
        {
            SetCanvasGroupActive(settingPageGroup, true);
            isSettingOn = true;
        }
        else
        {
            SetCanvasGroupActive(settingPageGroup, false);
            isSettingOn = false;
        }
    }

    /// <summary>
    /// 책 덮기 애니메이션 진행
    /// </summary>
    public void CloseBook()
    {
        SetCanvasGroupActive(newGamePageGroup, false);
        
        animator.SetTrigger("isClosed");
    }
    
    public IEnumerator FadeInTitleGroup()
    {
        SoundManager.Instance.PlaySfx(bookOpenSound,transform.position,false);
        
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
    
    /// <summary>
    /// 해당되는 Canvas Group의 상호작용 및 보이기 제거
    /// </summary>
    /// <param name="group"> 캔버스 그룹 </param>
    /// <param name="active"></param>
    void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
    
    /// <summary>
    /// Canvas Group의 색이 Fade out/in으로 점차 사라지게끔 만드는 코루틴 
    /// </summary>
    /// <param name="group"></param>
    /// <param name="duration"></param>
    /// <param name="fadeIn"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
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