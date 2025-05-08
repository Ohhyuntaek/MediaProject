using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum StageType { Normal, Boss, Shop }

[CreateAssetMenu(fileName = "NewStageData", menuName = "SO/Stage Data")]

public class StageData : ScriptableObject
{
    [Title("스테이지 정보")] 
    [SerializeField, LabelText("스테이지 이름")] private string _stageName;
    [SerializeField, LabelText("스테이지 타입")] private StageType _stageType;
    [SerializeField, LabelText("첫 스폰까지 걸리는 시간")] private float _startSpawnDelay;
    [SerializeField, LabelText("각 Dark가 스폰되는 딜레이")] private float _spawnTerm;
    [SerializeField, LabelText("스폰할 Darks 목록")] private List<GameObject> _darks;
    [SerializeField, LabelText("스폰되는 Dark의 수")] private int _darksCount;
    
    public string StageName => _stageName;
    public StageType StageType => _stageType;
    public float StartSpawnDelay => _startSpawnDelay;
    public float SpawnTerm => _spawnTerm;
    public List<GameObject> Darks => _darks;
    public int DarksCount => _darksCount;
}
