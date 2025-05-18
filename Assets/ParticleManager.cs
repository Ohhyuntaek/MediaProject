using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class AllyParticlePair
{
    public AllyType allyType;
    public ParticleSystem particleSystem;
}

[Serializable]
public class AllySkillParticleGroup
{
    public AllyType allyType;
    [Tooltip("이 스킬 타입에 사용할 파티클을 여러 개 할당하세요")]
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [Header("일반 공격 파티클 (단일)")]
    [SerializeField]
    private List<AllyParticlePair> _particlePairs = new List<AllyParticlePair>();

    [Header("스킬 파티클 (다중)")]
    [SerializeField]
    private List<AllySkillParticleGroup> _skillParticleGroups = new List<AllySkillParticleGroup>();

    private Dictionary<AllyType, ParticleSystem> _attackDictionary;
    private Dictionary<AllyType, List<ParticleSystem>> _skillDictionary;
    
    [SerializeField] private GameObject _vignetteObject;
    [Tooltip("비네팅 지속시간")]
    [SerializeField] private float _vignetteDuration = 2f;
    
    [Header("Shake 설정")]
    [Tooltip("흔들릴 카메라 Transform")]
    [SerializeField] private Transform _cameraTransform;
    [Tooltip("흔들림 세기")]
    [SerializeField] private float _shakeMagnitude = 0.1f;
    [Tooltip("흔들림 업데이트 간격")]
    [SerializeField] private float _shakeInterval = 0.02f;
    
    private bool _isVignetteActive = false;
    private Vector3 _cameraOriginalPos;

    private void Awake()
    {
        // 싱글턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // 일반 공격 딕셔너리 초기화
        _attackDictionary = new Dictionary<AllyType, ParticleSystem>();
        foreach (var pair in _particlePairs)
        {
            if (!_attackDictionary.ContainsKey(pair.allyType) && pair.particleSystem != null)
                _attackDictionary.Add(pair.allyType, pair.particleSystem);
        }
        if (_cameraTransform != null)
            _cameraOriginalPos = _cameraTransform.localPosition;
        // 스킬 딕셔너리 초기화 (여러 파티클)
        _skillDictionary = new Dictionary<AllyType, List<ParticleSystem>>();
        foreach (var group in _skillParticleGroups)
        {
            if (!_skillDictionary.ContainsKey(group.allyType))
                _skillDictionary.Add(group.allyType, group.particleSystems);
        }
    }

    /// <summary>
    /// 일반 공격용 단일 파티클을 재생합니다.
    /// </summary>
    public void PlayAttackParticle(AllyType type, Vector3 position)
    {
        if (_attackDictionary.TryGetValue(type, out var prefab) && prefab != null)
        {
            SpawnAndPlay(prefab, position);
        }
        else
        {
            Debug.LogWarning($"[ParticleManager] 일반 공격 파티클이 없습니다: {type}");
        }
    }

    /// <summary>
    /// 스킬용 파티클 중, 인덱스에 해당하는 하나를 재생합니다.
    /// </summary>
    public void PlaySkillParticle(AllyType type, Vector3 position, int index = 0)
    {
        // 1) 해당 타입으로 그룹이 있는지 체크
        if (!_skillDictionary.TryGetValue(type, out var list))
        {
            Debug.LogWarning($"[ParticleManager] 스킬 파티클 그룹이 없습니다: {type}");
            return;
        }

        // 2) 인덱스 범위 체크
        if (index < 0 || index >= list.Count)
        {
            Debug.LogWarning($"[ParticleManager] {type} 파티클 인덱스({index})가 범위를 벗어났습니다. [0 ~ {list.Count-1}]");
            return;
        }

        // 3) 실제로 하나만 뽑아서 출력
        var prefab = list[index];
        if (prefab == null)
        {
            Debug.LogWarning($"[ParticleManager] {type} 스킬 파티클[{index}]가 null 입니다.");
            return;
        }

        Debug.Log($"[ParticleManager] PlaySkillParticle → type: {type}, index: {index}, prefab: {prefab.name}");
        SpawnAndPlay(prefab, position);
    }

    /// <summary>
    /// 스킬용 파티클을 그룹에 등록된 전부를 순서대로 재생합니다.
    /// </summary>
    public void PlayAllSkillParticles(AllyType type, Vector3 position)
    {
        if (_skillDictionary.TryGetValue(type, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    SpawnAndPlay(list[i], position);
            }
        }
        else
        {
            Debug.LogWarning($"[ParticleManager] 스킬 파티클 그룹이 없습니다: {type}");
        }
    }

   
    private void SpawnAndPlay(ParticleSystem prefab, Vector3 position)
    {
        var ps = Instantiate(prefab, position, Quaternion.identity);
        ps.Play();

        var main = ps.main;
        float lifetime = main.duration;
        Destroy(ps.gameObject, lifetime);
    }
    #region Vignette & Shake

    /// <summary>
    /// 비네팅 + 카메라 흔들림 효과를 트리거합니다.
    /// 이미 동작 중이면 무시됩니다.
    /// </summary>
    public void TriggerVignetteAndShake()
    {
        if (_isVignetteActive || _vignetteObject == null || _cameraTransform == null)
            return;

        StartCoroutine(VignetteAndShakeRoutine());
    }

    private IEnumerator VignetteAndShakeRoutine()
    {
        _isVignetteActive = true;
        _vignetteObject.SetActive(true);

        
        StartCoroutine(CameraShakeRoutine(_vignetteDuration));

        
        yield return new WaitForSeconds(_vignetteDuration);

        _vignetteObject.SetActive(false);

        
        yield return new WaitForSeconds(0.1f);
        _isVignetteActive = false;
    }

    private IEnumerator CameraShakeRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            _cameraTransform.localPosition = _cameraOriginalPos + UnityEngine.Random.insideUnitSphere * _shakeMagnitude;
            elapsed += _shakeInterval;
            yield return new WaitForSeconds(_shakeInterval);
        }
        // 원위치 복원
        _cameraTransform.localPosition = _cameraOriginalPos;
    }

    #endregion
}
