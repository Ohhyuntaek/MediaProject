using Microsoft.Unity.VisualStudio.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "SO/Player Data")]
public class DawnData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string _playerName;
    [SerializeField] private Sprite _portrait;
    [SerializeField] private float _maxHP;
    [SerializeField] private float _activeSkillCooldown;
    [SerializeField] private AllySkillType _activeSkillType;
    [SerializeField] private UnitTribe _unitTribe;
    [SerializeField] private int _maxEnergy;    
    [SerializeField] private int _initialEnergy;
    [SerializeField] private int _energyUseValue;
    [SerializeField] private int _chargespeed; 
        
    [Header("Prefab 정보 추가")] 
    [SerializeField] private GameObject _prefab;
    
    [Title("사운드")]
    [SerializeField, LabelText("공격사운드")] private AudioClip attackSfx;
    [SerializeField, LabelText("죽을때 나는 소리")] private AudioClip deathSfx;
    [SerializeField, LabelText("스킬을 쓸때 나는 소리")] private AudioClip skillSfx;
    
    
    

    public string PlayerName { get => _playerName; set => _playerName = value; }
    public Sprite Portrait { get => _portrait; set => _portrait = value; }
    public float MaxHP { get => _maxHP; set => _maxHP = value; }
    public float ActiveSkillCooldown { get => _activeSkillCooldown; set => _activeSkillCooldown = value; }
    public AllySkillType ActiveSkillType { get => _activeSkillType; set => _activeSkillType = value; }
    public int MaxEnergy { get => _maxEnergy; set => _maxEnergy = value; }
    public int InitialEnergy { get => _initialEnergy; set => _initialEnergy = value; }
    public int ChargingSpd { get => _chargespeed; set => _chargespeed = value; }
    public UnitTribe Tribe { get => _unitTribe; set => _unitTribe = value; }
    public GameObject Prefab => _prefab;
    public AudioClip AttackSound => attackSfx;
    public AudioClip DeathSound => deathSfx;
    public AudioClip SkillSound => skillSfx;
    public int energyUseValue => _energyUseValue;
}
