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
    [SerializeField] private float _moveSpeed = 0.5f; // 초당 0.5칸 이동
    [SerializeField] private int _maxHp;
    [SerializeField] private float _atkSpeed;         // 초당 공격 횟수
    [SerializeField] private int _damage;
    [SerializeField] private int _range;           // 공격력
    

    [Header("스킬 및 타입")]
    [SerializeField] private EnemySkill _enemySkill;
    [SerializeField] private EnemyType _enemyType;

    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public float AtkSpeed { get => _atkSpeed; set => _atkSpeed = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public int ATKRange{get => _range; set => _range = value; }
    public EnemySkill Skill { get => _enemySkill; set => _enemySkill = value; }
    public EnemyType Type { get => _enemyType; set => _enemyType = value; }
}