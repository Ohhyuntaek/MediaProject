using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "SO/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string _playerName;
    [SerializeField] private float _maxHP;
    [SerializeField] private float _activeSkillCooldown;
    [SerializeField] private AllySkillType _activeSkillType;
    [SerializeField] private UnitTribe _unitTribe;
    
    
    [SerializeField] private int _maxEnergy;    
    [SerializeField] private int _initialEnergy;
    [SerializeField] private int _chargespeed; 
    

    public string PlayerName { get => _playerName; set => _playerName = value; }
    public float MaxHP { get => _maxHP; set => _maxHP = value; }
    public float ActiveSkillCooldown { get => _activeSkillCooldown; set => _activeSkillCooldown = value; }
    public AllySkillType ActiveSkillType { get => _activeSkillType; set => _activeSkillType = value; }
    public int MaxEnergy { get => _maxEnergy; set => _maxEnergy = value; }
    public int InitialEnergy { get => _initialEnergy; set => _initialEnergy = value; }
    public int ChargingSpd { get => _chargespeed; set => _chargespeed = value; }
    public UnitTribe Tribe { get => _unitTribe; set => _unitTribe = value; }
}
