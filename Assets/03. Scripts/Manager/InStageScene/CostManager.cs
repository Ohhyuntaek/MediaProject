using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    [Header("코스트 표시 텍스트")]
    [SerializeField] private TMP_Text costText;

    private bool isStopCostUp = false;
    private float costTimer = 0;
    private float totalCost = 0f;

    private float costMultiplier => RuntimeDataManager.Instance.enhancement.costMultiplier;
    public float TotalCost
    {
        get => totalCost;
        set
        {
            totalCost = Mathf.Clamp(value, 0f, 99f);
            costTimer = totalCost / costMultiplier; // 내부 timer 값도 갱신
            UpdateCostText();
        }
    }

    private void Start()
    {
        costTimer = totalCost / costMultiplier; // 내부 timer 값도 갱신
    }

    private void Update()
    {
        UpdateCostText();
    }

    /// <summary>
    /// 코스트 증가 코루틴
    /// </summary>
    public IEnumerator IncreaseCost()
    {
        while (!isStopCostUp)
        {
            if (totalCost < 99f)
            {
                costTimer++;
                totalCost = Mathf.Min(costTimer * costMultiplier, 99f);
                UpdateCostText();
            }

            yield return new WaitForSeconds(1f);
        }
    }
    
    /// <summary>
    /// 코스트 감소 함수
    /// </summary>
    /// <param name="consumedCost">소모되는 코스트 값</param>
    public void DecreaseCost(float consumedCost)
    {
        totalCost = Mathf.Max(0f, totalCost - consumedCost);
        UpdateCostText();
    }

    public void StopCostUP(bool check)
    {
        isStopCostUp = check;
    }

    /// <summary>
    /// 코스트 텍스트 업데이트
    /// </summary>
    private void UpdateCostText()
    {
        costText.text = totalCost.ToString("F0");
    }
}
