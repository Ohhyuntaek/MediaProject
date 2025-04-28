using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SFX 재생용 AudioSource 프리팹")]
    [SerializeField] private AudioSource _sfxSourcePrefab;

    [Header("동시 재생 제한")]
    [Tooltip("같은 클립을 동시에 이 개수 이상 재생하지 않습니다.")]
    [SerializeField] private int _maxSimultaneousPerClip = 5;

    // 풀과 재생 카운터
    private List<AudioSource> _pool = new List<AudioSource>();
    private Dictionary<AudioClip,int> _playingCount = new Dictionary<AudioClip,int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 지정한 SFX를 월드 좌표에서 재생합니다.
    /// 같은 클립이 이미 _maxSimultaneousPerClip 이상 재생 중이면 호출을 무시합니다.
    /// </summary>
    public void PlaySfx(AudioClip clip, Vector3 worldPos, bool spatial = true)
    {
        if (clip == null) return;

        // 현재 재생 중인 개수 체크
        if (!_playingCount.TryGetValue(clip, out int count)) 
            count = 0;

        if (count >= _maxSimultaneousPerClip)
            return;

        // 카운터 증가
        _playingCount[clip] = count + 1;

        // AudioSource 가져오기
        var src = GetOrCreateSource();
        src.transform.position = worldPos;
        src.spatialBlend = spatial ? 1f : 0f;
        src.PlayOneShot(clip);

        // 재생이 끝난 뒤 카운터 감소
        StartCoroutine(DecrementAfter(clip, clip.length));
    }

    private AudioSource GetOrCreateSource()
    {
        
        foreach (var src in _pool)
        {
            if (!src.isPlaying)
                return src;
        }
        
        var inst = Instantiate(_sfxSourcePrefab, transform);
        _pool.Add(inst);
        return inst;
    }

    private IEnumerator DecrementAfter(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playingCount.TryGetValue(clip, out int count) && count > 0)
        {
            _playingCount[clip] = count - 1;
        }
    }
}
