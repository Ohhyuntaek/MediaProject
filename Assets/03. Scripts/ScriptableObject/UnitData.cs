using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public enum UnitFaction { Ally, Enemy, Player }
public enum UnitTribe{Person,}
public enum UnitType { Front, Middle, Rear }
public enum DamageType { Physical, Magical }
public enum AllySkillType { None, MovementBlock, DamageDealer, Debuff, AllyBuff,KnockBack,NightLord }
public enum EnemySkillType { None, AllySupporter, AllyDisruptor, PlayerDebuffer }
public enum PlayerSkillType{Test1,Test2,Test3}
public enum TargetingType { Single, Area }

[CreateAssetMenu(fileName = "NewUnitData", menuName = "SO/Unit Data")]
public class UnitData : ScriptableObject
{
    [Title("기본 유닛 정보")]
    [SerializeField, LabelText("유닛 이름")] private string _unitName;
    [SerializeField, LabelText("소속")] private UnitFaction _unitFaction;
    [SerializeField, LabelText("배치 타입")] private UnitType _unitType;
    [SerializeField, LabelText("피해 타입")] private DamageType _damageType;
    [SerializeField, LabelText("유닛 종족")] private UnitTribe _unitTribe;
    [SerializeField, LabelText("부활 가능 여부")] private bool _canRevive;
    
    [FormerlySerializedAs("_skillCooltime")]
    [Title("스킬 관련 공통")]
    [SerializeField, LabelText("스킬 쿨타임")] private float _skillCoolTime;
    [SerializeField, LabelText("스킬 이름")] private string _skillname;
    [SerializeField, LabelText("스킬 관련 설명, 추후 툴팁 개발 가능성")] private string _skilldescripter;
    [SerializeField, LabelText("스킬 이펙트")] private List<GameObject> _skillEffect;
    [SerializeField, LabelText("스킬 범위/단일")] private TargetingType _targetingType;
    [SerializeField, LabelText("스킬/공격 사정거리 ")] private int _attackRange;
    

    [ShowIf(nameof(IsAlly)), LabelText("아군 스킬 타입")]
    [SerializeField] private AllySkillType _allySkillType;

    [ShowIf(nameof(IsEnemy)), LabelText("적군 스킬 타입")]
    [SerializeField] private EnemySkillType _enemySkillType;

    [Title("공통 스탯")]
    [SerializeField, LabelText("공격력")] private int _baseAttack;
    [SerializeField, LabelText("공격 속도")] private float _attackSpeed;
    [SerializeField, LabelText("최대 체력")] private float _maxHP;
    [SerializeField, LabelText("이동 가능 여부")] private bool _canMove;
    

    [Title("플레이어 전용 스탯"), ShowIf(nameof(IsPlayer))] 
    [SerializeField, LabelText("플레이어 스킬 타입")] private PlayerSkillType _playerSkillType;

    [Title("아군 전용 스탯"), ShowIf(nameof(IsAlly))]
    [SerializeField, LabelText("유지 시간")] private float _duration;

    [Title("적군 전용 스탯"), ShowIf(nameof(IsEnemy))]
    [SerializeField, LabelText("이동 속도")] private float _moveSpeed;

    
    public string UnitName => _unitName;
    public UnitFaction UnitFaction => _unitFaction;
    public UnitType UnitType => _unitType;
    public DamageType DamageType => _damageType;
    public AllySkillType AllySkillType => _allySkillType;
    public PlayerSkillType PlayerSkillType => _playerSkillType;
    public EnemySkillType EnemySkillType => _enemySkillType;
    public int BaseAttack => _baseAttack;
    public float AttackSpeed => _attackSpeed;
    public float MaxHP => _maxHP;
    public bool CanMove => _canMove;
    public float Duration => _duration;
    public float MoveSpeed => _moveSpeed;
    public float SkillCoolTime => _skillCoolTime;
    public List<GameObject> SkillEffect => _skillEffect;
    public string SkillName => _skillname;
    public string SkillDescriptor => _skilldescripter;
    public TargetingType TargetingType => _targetingType;
    public int AttackRange => _attackRange;
    public UnitTribe UnitTribe => _unitTribe;
    public bool CanRevive => _canRevive;
    private bool IsAlly() => _unitFaction == UnitFaction.Ally;
    private bool IsEnemy() => _unitFaction == UnitFaction.Enemy;
    private bool IsPlayer() => _unitFaction == UnitFaction.Player;
}
