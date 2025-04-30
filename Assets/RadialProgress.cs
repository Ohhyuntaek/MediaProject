using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    public TMP_Text ProgressIndicator;
    public Image LoadingBar;

    private Dawn targetDawn;

    private void Start()
    {
        targetDawn = FindObjectOfType<Dawn>();  // Dawn 객체 찾기 (씬에 하나만 있을 경우)
        if (targetDawn == null)
        {
            Debug.LogError("Dawn을 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (targetDawn == null) return;

        // Dawn에서 가져온 쿨다운 진행률 (0 ~ 1)
        float cooldownRatio = targetDawn.ActiveCooldownRatio;

        // UI 업데이트
        LoadingBar.fillAmount = cooldownRatio;
        if (cooldownRatio < 1f)
        {
            ProgressIndicator.text = Mathf.RoundToInt(cooldownRatio * 100) + "%";
        }
        else
        {
            ProgressIndicator.text = "Ready!";
        }
    }
}