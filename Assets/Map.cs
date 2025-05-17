using System.Collections;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private CanvasGroup mapCanvasGroup;
    
    [SerializeField] private float delayAfterMapOppend = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapAnimator = GetComponent<Animator>();
        SetCanvasGroupActive(mapCanvasGroup, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeInMap()
    {
        // MapOpen 애니메이션의 이벤트로 추가 - 책이 펼쳐지면 delayAfterMapOppend 시간 후 mapCanvasGroup 생성
        yield return new WaitForSeconds(delayAfterMapOppend);
        yield return StartCoroutine(FadeCanvasGroup(mapCanvasGroup, fadeDuration, true));
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
