using Sirenix.OdinInspector;
using UnityEngine;

public class Enhancement : MonoBehaviour
{
    [Header("유닛 외 강화 수치")]
    [LabelText("코스트 상승 속도")] public float costMultiplier = 1f;
    [LabelText("카드 스폰 속도")] public float cardSpawnIntervalMultiplier = 2f;
    [LabelText("액티브 스킬 쿨다운 수치")] public float cooldownMultiplier = 1f;  // 쿨다운 속도에 적용될 배수 (1보다 작을수록 빠름)
    [LabelText("액티브 스킬 에너지 충전 속도")] public float energyChargeMultiplier = 1f; // 에너지 회복 배수 (1보다 클수록 빠름)
    
    [Header("유닛 강화 수치")]
    [LabelText("전열 공격력 수치")] public float increaseFLAttackPoint = 0f;
    [LabelText("중열 공격력 수치")] public float increaseMLAttackPoint = 0f;
    [LabelText("후열 공격력 수치")] public float increaseRLAttackPoint = 0f;
        
    /// <summary>
    /// 액티브 스킬 쿨다운 수치 설정
    /// </summary>
    public float CooldownMultiplier
    {
        get => cooldownMultiplier;
        set => cooldownMultiplier = Mathf.Clamp(value, 0.1f, 10f); // 0.1x ~ 10x 제한
    }
    
    /// <summary>
    /// 액티브 스킬 에너지 충전 속도 설정
    /// </summary>
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
