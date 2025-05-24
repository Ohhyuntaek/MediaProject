using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DawnCoolTimeProgress : MonoBehaviour
{
    [SerializeField]
    private TMP_Text progressIndicator;
    [SerializeField]
    private Image loadingBar;
    [SerializeField]
    private float blinkInterval = 0.5f;

    private Dawn targetDawn;
    private float blinkTime = 0;

    /// <summary>
    /// 외부에서 Dawn 객체를 지정할 수 있게 해주는 Setter
    /// </summary>
    public void SetTargetDawn(Dawn dawn)
    {
        targetDawn = dawn;
    }

    private void Start()
    {
        float cooldownRatio = 0;
    }
    
    private void Update()
    {
        if (targetDawn == null) return;

        // Dawn에서 가져온 쿨다운 진행률 (0 ~ 1)
        float cooldownRatio = targetDawn.ActiveCooldownRatio;

        // UI 업데이트
        loadingBar.fillAmount = cooldownRatio;
        if (cooldownRatio < 1f)
        {
            progressIndicator.text = Mathf.RoundToInt(cooldownRatio * 100) + "%";
        }
        else
        {
            progressIndicator.text = "Space!";
        }
    }
}