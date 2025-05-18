using UnityEngine;

public class Enhancement : MonoBehaviour
{
    /// <summary>
    /// 코스트 가속 값
    /// </summary>
    public float costMultiplier = 1f;
    public float cardSpawnIntervalMultiplier = 2f;
    public float cooldownMultiplier = 1f;  // 쿨다운 속도에 적용될 배수 (1보다 작을수록 빠름)
    public float energyChargeMultiplier = 1f; // 에너지 회복 배수 (1보다 클수록 빠름)

    // 외부에서 접근할 수 있도록 프로퍼티
    public float CooldownMultiplier
    {
        get => cooldownMultiplier;
        set => cooldownMultiplier = Mathf.Clamp(value, 0.1f, 10f); // 0.1x ~ 10x 제한
    }
    
    public float EnergyChargeMultiplier
    {
        get => energyChargeMultiplier;
        set => energyChargeMultiplier = Mathf.Clamp(value, 0.1f, 10f);
    }
    
    /// <summary>
    /// 코스트가 증가하는 속도 가속 함수
    /// </summary>
    /// <param name="multiplier">가속할 값</param>
    public void CostSpeedUp(float multiplier)
    {
        costMultiplier *= multiplier;
    }
    
    /// <summary>
    /// 코스트 가속 값 설정
    /// </summary>
    /// <param name="multiplier">가속할 값 (기본값 1f)</param>
    public void SetCostMultiplier(float multiplier = 1f)
    {
        costMultiplier = multiplier;
    }

    
    /// <summary>
    /// 다음 카드가 스폰하기까지 걸리는 시간을 감소하는 함수
    /// </summary>
    /// <param name="differenceValue">감소할 값</param>
    public void CardSpawnSpeedUP(float differenceValue)
    {
        cardSpawnIntervalMultiplier -= differenceValue;
    }
    
}
