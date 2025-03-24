using UnityEngine;

public enum UnitFaction { Ally, Enemy }
public enum UnitType { Front, Middle, Rear }
public enum DamageType { Physical, Magical }

[CreateAssetMenu(fileName = "NewUnitData", menuName = "SO/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string unitName;
    public UnitFaction faction; // 아군 / 적군
    public UnitType unitType;   // 전열 / 중열 / 후열
    public DamageType damageType;

    [Header("스탯")]
    public float baseAttack;
    public float attackSpeed;
    public float maxHP;

    [Tooltip("아군 유닛만 해당")]
    public float duration;  // 아군 유닛: 유지 시간

    [Tooltip("적군 유닛만 해당")]
    public float moveSpeed; // 적 유닛: 이동 속도

    [Header("행동 타입")]
    public bool canMove; // 적군은 true, 아군은 false

    [Header("특수 능력 (적군 한정)")]
    public bool isSupporter;  // 다른 적군 이동속도 증가
    public bool isDisruptor;  // 아군 유닛 파괴
    public bool isDebuffer;   // 플레이어 스킬 제한

    [Header("특성 칸 강화 관련")]
    public int[] preferredCellIndices; // 해당 위치에 소환 시 강화
}
