using UnityEngine;
using Sirenix.OdinInspector;

public enum RemnantType
{
    Unit,       // 모든 유닛에게
    JoanDarc,   // 특정 유닛 예시
    NightLord,
    BountyHunter,
    Rogue,
    CentaurLady,
    Salamander
}

public enum RemnantStatType
{
    ATK,
    DURATION,
    SPD
}

[CreateAssetMenu(fileName = "newRemnantSO", menuName = "SO/Remnant")]
public class RemnantSO : ItemEffectBase
{
    [SerializeField, LabelText("Remnant Type: Unit = 모두에게 적용")]
    private RemnantType      _remnantType;

    [SerializeField, LabelText("증가시킬 스탯 종류")]
    private RemnantStatType  _remnantStat;

    [SerializeField, LabelText("스텟 증가량")]
    private int              _amount;

    public RemnantType     Type   => _remnantType;
    public RemnantStatType Stat   => _remnantStat;
    public int             Amount => _amount;

    public override void Apply()
    {
      
    }
}