using UnityEngine;
using UnityEngine.Serialization;

public enum EnhancementType
{
    CostUp,        // 코스트 증가 속도 향상
    CardSpawnSpeedUp,   // 카드 생성 속도 향상
    CooldownSpeedUp,    // 쿨타임 감소 속도 증가
    EnergyChargeSpeedUp  // 에너지 충전 속도 증가
}

[CreateAssetMenu(fileName = "NewEnhancementCardData", menuName = "SO/Enhancement Card Data")]
public class EnhancementCardData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string _enhancementCardName;
    [SerializeField] private string _description;
    
    [Header("강화 효과")]
    [SerializeField] private EnhancementType _enhancementType;
    [SerializeField] private float _minRangeValue = 1.2f;
    [SerializeField] private float _maxRangeValue = 1.5f;
    
    public string EnhancementCardName => _enhancementCardName;
    public string Description => _description;
    public EnhancementType EnhancementType => _enhancementType;
    public float MinRangeValue => _minRangeValue;
    public float MaxRangeValue => _maxRangeValue;
}
