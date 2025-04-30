using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    public TMP_Text ProgressIndicator;
    public Image LoadingBar;

    private Dawn targetDawn;

    /// <summary>
    /// 외부에서 Dawn 객체를 지정할 수 있게 해주는 Setter
    /// </summary>
    public void SetTargetDawn(Dawn dawn)
    {
        targetDawn = dawn;
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
            ProgressIndicator.text = "준비!";
        }
    }
}