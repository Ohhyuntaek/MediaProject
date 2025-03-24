using UnityEngine;
using Sirenix.OdinInspector;

public enum UnitFaction { Ally, Enemy, Player }
public enum UnitType { Front, Middle, Rear }
public enum DamageType { Physical, Magical }
public enum AllySkillType { None, MovementBlock, DamageDealer, Debuff, AllyBuff }
public enum EnemySkillType { None, AllySupporter, AllyDisruptor, PlayerDebuffer }

[CreateAssetMenu(fileName = "NewUnitData", menuName = "SO/Unit Data")]
public class UnitData : ScriptableObject
{
    [Title("기본 유닛 정보")]
    [SerializeField, LabelText("유닛 이름")] private string _unitName;
    [SerializeField, LabelText("소속")] private UnitFaction _unitFaction;
    [SerializeField, LabelText("배치 타입")] private UnitType _unitType;
    [SerializeField, LabelText("피해 타입")] private DamageType _damageType;

    [ShowIf(nameof(IsAlly)), LabelText("아군 스킬 타입")]
    [SerializeField] private AllySkillType _allySkillType;

    [ShowIf(nameof(IsEnemy)), LabelText("적군 스킬 타입")]
    [SerializeField] private EnemySkillType _enemySkillType;

    [Title("공통 스탯")]
    [SerializeField, LabelText("공격력")] private float _baseAttack;
    [SerializeField, LabelText("공격 속도")] private float _attackSpeed;
    [SerializeField, LabelText("최대 체력")] private float _maxHP;
    [SerializeField, LabelText("이동 가능 여부")] private bool _canMove;

    [Title("🟦 아군 전용 스탯"), ShowIf(nameof(IsAlly))]
    [SerializeField, LabelText("유지 시간")] private float _duration;

    [Title("🟥 적군 전용 스탯"), ShowIf(nameof(IsEnemy))]
    [SerializeField, LabelText("이동 속도")] private float _moveSpeed;

    
    public string UnitName => _unitName;
    public UnitFaction UnitFaction => _unitFaction;
    public UnitType UnitType => _unitType;
    public DamageType DamageType => _damageType;
    public AllySkillType AllySkillType => _allySkillType;
    public EnemySkillType EnemySkillType => _enemySkillType;
    public float BaseAttack => _baseAttack;
    public float AttackSpeed => _attackSpeed;
    public float MaxHP => _maxHP;
    public bool CanMove => _canMove;
    public float Duration => _duration;
    public float MoveSpeed => _moveSpeed;

    
    private bool IsAlly() => _unitFaction == UnitFaction.Ally;
    private bool IsEnemy() => _unitFaction == UnitFaction.Enemy;
}
