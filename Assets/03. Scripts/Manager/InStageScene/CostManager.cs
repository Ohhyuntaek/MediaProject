using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    [Header("기본 코스트 증가 주기 (초)")]
    [SerializeField] private float baseCostUpSpeed = 2f; // costMultiplier가 1일 때의 기본 속도

    [Header("코스트 표시 텍스트")]
    [SerializeField] private TMP_Text costText;

    private bool isStopCostUp = false;
    private float totalCost = 0f;

    // 코스트 최대값
    private const float maxCost = 99f;

    // 외부 강화 시스템에서 가져오는 배수
    private float costMultiplier => RuntimeDataManager.Instance.enhancement.costMultiplier;

    // 실제 코스트 증가 주기 = 기본값 ÷ 배수
    private float AdjustedCostUpSpeed => baseCostUpSpeed / costMultiplier;
    
    public float TotalCost
    {
        get => totalCost;
        set
        {
            totalCost = Mathf.Clamp(value, 0f, maxCost);
            UpdateCostText();
        }
    }

    private void Start()
    {
        UpdateCostText();
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
            if (totalCost < maxCost)
            {
                totalCost = Mathf.Min(totalCost + 1f, maxCost);
                UpdateCostText();
            }

            yield return new WaitForSeconds(AdjustedCostUpSpeed);
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
