using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum EnemySkill
{
    None,
    Basic3
}

public enum EnemyType
{
    Basic,
    Special,
    Boss
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "SO/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 능력치")] 
    [SerializeField] private string _enemyName;
    [SerializeField] private float _moveSpeed = 0.5f; 
    [SerializeField] private int _maxHp;
    [SerializeField] private float _atkSpeed;        
    [SerializeField] private int _damage;
    [SerializeField] private int _range;
    [SerializeField] private float _defense;          
    
    [Title("사운드")]
    [SerializeField, LabelText("공격사운드")] private AudioClip[] attackSfx;
    [SerializeField, LabelText("죽을때 나는 소리")] private AudioClip[] deathSfx;
    [SerializeField, LabelText("스킬을 쓸때 나는 소리")] private AudioClip[] skillSfx;

    [Header("스킬 및 타입")]
    [SerializeField] private EnemySkill _enemySkill;
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private List<GameObject> _skillEffets;

    public string EnemyName => _enemyName;
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public float AtkSpeed { get => _atkSpeed; set => _atkSpeed = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public int ATKRange{get => _range; set => _range = value; }
    public float Deffense { get => _defense; set => _defense = value; }
    public List<GameObject> EnemyEffect => _skillEffets;
    public EnemySkill Skill { get => _enemySkill; set => _enemySkill = value; }
    public EnemyType Type { get => _enemyType; set => _enemyType = value; }
    public AudioClip[] AttackSound => attackSfx;
    public AudioClip[] DeathSound => deathSfx;
    public AudioClip[] SkillSound => skillSfx;
}