using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public enum UnitFaction { Ally,Player }
public enum UnitTribe{Person,}
public enum UnitType { Front, Mid, Rear }
public enum AllyType { JoanDarc, NightLord, BountyHunter, Rogue, CentaurLady, Salamander,Killren,Aura,Diabunny }
public enum DamageType { Physical, Magical }
public enum AllySkillType { None, CentaurLadySkill, BountyHunterSkill, SalamenderSkill, JoanDarcSkill, NightLordSkill,KillrenSkill,AuraSkill,DiabunnySkill }
public enum PlayerSkillType{Test1,Test2,Test3}
public enum TargetingType { Single, Area }

[CreateAssetMenu(fileName = "NewAllyData", menuName = "SO/Ally Data")]
public class UnitData : ScriptableObject
{
    [Title("기본 유닛 정보")]
    [SerializeField, LabelText("유닛 이름")] private string _unitName;
    [SerializeField, LabelText("소속")] private UnitFaction _unitFaction;
    [SerializeField, LabelText("배치 타입")] private UnitType _unitType;
    [SerializeField, LabelText("Ally 타입")] private AllyType _allyType;
    [SerializeField, LabelText("피해 타입")] private DamageType _damageType;
    [SerializeField, LabelText("유닛 종족")] private UnitTribe _unitTribe;
    [SerializeField, LabelText("부활 가능 여부")] private bool _canRevive;
    [SerializeField, LabelText("스킬/공격 사정거리 ")] private int _attackRange;
    [SerializeField,LabelText("공격 사정거리 So ")] private DetectionPatternSO _detectionPattern;
    [SerializeField,LabelText("specialattack 사정거리 So ")] private DetectionPatternSO _skilldetectionPattern;

    
    [FormerlySerializedAs("_skillCooltime")]
    [Title("스킬 관련 공통")]
    [SerializeField, LabelText("스킬 쿨타임")] private float _skillCoolTime;
    [SerializeField, LabelText("스킬 이름")] private string _skillname;
    [SerializeField, LabelText("스킬 이펙트")] private List<GameObject> _skillEffect;
    [SerializeField, LabelText("스킬 범위/단일")] private TargetingType _targetingType;

    [ShowIf(nameof(IsAlly)), LabelText("아군 스킬 타입")]
    [SerializeField] private AllySkillType _allySkillType;
    
    [Title("공통 스탯")]
    [SerializeField, LabelText("공격력")] private int _baseAttack;
    [SerializeField, LabelText("공격 속도")] private float _attackSpeed;
    [SerializeField, LabelText("최대 체력")] private float _maxHP;
    [SerializeField, LabelText("코스트")] private int cost;

    [FormerlySerializedAs("_image")]
    [Title("툴팁관련")] 
    [SerializeField, LabelText("초상화")] private Sprite _sprite;
    [SerializeField, LabelText("추천 텍스트")] private string _recomendText;
    [SerializeField, LabelText("일반공격 텍스트")] private string _nomalText;
    [SerializeField, LabelText("스킬 관련 설명, 추후 툴팁 개발 가능성")] private string _skilldescripter;
       
    
    [Title("사운드")]
    [SerializeField, LabelText("공격사운드")] private AudioClip[] attackSfx;
    [SerializeField, LabelText("죽을때 나는 소리")] private AudioClip[] deathSfx;
    [SerializeField, LabelText("스킬을 쓸때 나는 소리")] private AudioClip[] skillSfx;
    [Title("플레이어 전용 스탯"), ShowIf(nameof(IsPlayer))] 
    [SerializeField, LabelText("플레이어 스킬 타입")] private PlayerSkillType _playerSkillType;

    [Title("아군 전용 스탯"), ShowIf(nameof(IsAlly))]
    [SerializeField, LabelText("유지 시간")] private float _duration;


    public List<int> GetApplyItemUnitData(List<RemnantSO> remnantSo)
    {
        int copyAtk = _baseAttack;
        int copySpd = (int)_attackSpeed;
        int copyDuration = (int)_duration;
        List<int> _stat = new List<int>();
       
        if (remnantSo != null)
        {


            foreach (var item in remnantSo)
            {
                if (item.Type == RemnantType.Unit)
                {
                    switch (item.Stat)
                    {
                        case RemnantStatType.ATK:
                            copyAtk += item.Amount;
                      
                            break;
                        case RemnantStatType.SPD:
                            copySpd += item.Amount;
                           
                            break;
                        case RemnantStatType.DURATION:
                            
                            copyDuration += item.Amount;
                           
                            break;
                    }
                }
            }
            _stat.Add(copyAtk);
            _stat.Add(copySpd);
            _stat.Add(copyDuration);
        }

        return _stat;
    }
    public string UnitName => _unitName;
    public UnitFaction UnitFaction => _unitFaction;
    public UnitType UnitType => _unitType;
    public AllyType AllyType => _allyType;
    public DamageType DamageType => _damageType;
    public AllySkillType AllySkillType => _allySkillType;
    public PlayerSkillType PlayerSkillType => _playerSkillType;
    public int BaseAttack => _baseAttack;
    public float AttackSpeed => _attackSpeed;
    public float MaxHP => _maxHP;
    public float Duration => _duration;
    public float SkillCoolTime => _skillCoolTime;
    public List<GameObject> SkillEffect => _skillEffect;
    public string SkillName => _skillname;
    public string SkillDescriptor => _skilldescripter;
    public TargetingType TargetingType => _targetingType;
    public int AttackRange => _attackRange;
    public UnitTribe UnitTribe => _unitTribe;
    public bool CanRevive => _canRevive;
    private bool IsAlly() => _unitFaction == UnitFaction.Ally;
    private bool IsPlayer() => _unitFaction == UnitFaction.Player;
    public int Cost => cost;
    public Sprite Sprite => _sprite;
    public DetectionPatternSO DetectionPatternSo => _detectionPattern;
    public AudioClip[] AttackSound => attackSfx;
    public AudioClip[] DeathSound => deathSfx;
    public AudioClip[] SkillSound => skillSfx;
    public string NomalDescriptor => _nomalText;
    public string RecomendDescriptor => _recomendText;
    public DetectionPatternSO SkillDetectionPatternSo => _skilldetectionPattern;

}
