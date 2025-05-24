using Sirenix.OdinInspector;
using UnityEngine;

public enum AllyItemType {Add,Minus}

[CreateAssetMenu(fileName = "newAllyItem", menuName = "SO/Ally Item")]
public class AllyItemData : ItemEffectBase
{
    [SerializeField, LabelText("아이템 타입 (추가/감소)")]
    private AllyItemType _allyItemType;

    [SerializeField, LabelText("증가,감소시킬 ALLYTYPE")]
    private AllyType _allyType;
    [SerializeField, LabelText("가중치")] private int _weight;

    // 외부에서 읽기 전용으로 사용할 경우
    public AllyItemType ItemType => _allyItemType;
    public int Weight => _weight;

    public override void Apply()
    {

    }
}
    
