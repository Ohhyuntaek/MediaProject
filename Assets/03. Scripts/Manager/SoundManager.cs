using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SFX 재생용 AudioSource 프리팹")] [SerializeField]
    private AudioSource _sfxSourcePrefab;

    [Header("BGM 재생용 AudioSource 프리팹")] [SerializeField]
    private AudioSource _bgmSourcePrefab;

    [Header("동시 재생 제한 (SFX)")] [Tooltip("같은 클립을 동시에 이 개수 이상 재생하지 않습니다.")] [SerializeField]
    private int _maxSimultaneousPerClip = 5;

    // SFX 풀 및 재생 카운트
    private List<AudioSource> _sfxPool = new List<AudioSource>();
    private Dictionary<AudioClip, int> _playingSfxCount = new Dictionary<AudioClip, int>();

    // BGM 재생용
    private AudioSource _bgmSource;

    [Header("BGM 플레이리스트 (Inspector 에 Array 로 할당)")] [SerializeField]
    private List<AudioClip> _bgmPlaylist;

    private int _bgmIndex;
    private bool _bgmLoop;
    private Coroutine _bgmCoroutine;

    private Slider _sfxSlider;
    private Slider _bgmSlider;

    private float _sfxVolume = 1f;
    private float _bgmVolume = 1f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) 이전에 연결된 이벤트는 제거
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        if (_bgmSlider != null)
            _bgmSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);

        // 2) 씬에서 슬라이더들 찾아 연결 (태그나 이름, 직접 Find 등)
        var sfxGO = GameObject.FindWithTag("SFXSlider");
        _sfxSlider = sfxGO ? sfxGO.GetComponent<Slider>() : null;

        var bgmGO = GameObject.FindWithTag("BgmSlider");
        _bgmSlider = bgmGO ? bgmGO.GetComponent<Slider>() : null;

        // 3) 찾았으면 리스너 다시 추가
        if (_sfxSlider != null)
        {
            _sfxSlider.value=_sfxVolume;
            Debug.Log(_sfxVolume + "설정");
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.value = _bgmVolume;
            _bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }

       
    }


private void Awake()
    {
        // 싱글턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // BGM Source 셋업
        _bgmSource = Instantiate(_bgmSourcePrefab, transform);
        _bgmSource.loop = false; // 개별 트랙 루프는 리스트 로직에서 관리
    }

    //──────────────────────────────────────────────
    // SFX 재생
    public void PlaySfx(AudioClip clip, Vector3 worldPos, bool spatial = true)
    {
        if (clip == null) return;

        // 동시 재생 제한 체크
        _playingSfxCount.TryGetValue(clip, out int count);
        if (count >= _maxSimultaneousPerClip)
            return;

        _playingSfxCount[clip] = count + 1;

        // AudioSource 가져오기 혹은 생성
        var src = GetOrCreateSfxSource();
        src.transform.position = worldPos;
        src.spatialBlend       = spatial ? 1f : 0f;
        src.PlayOneShot(clip);

        // 재생 완료 후 카운트 감소
        StartCoroutine(DecrementSfxCountAfter(clip, clip.length));
    }

    private AudioSource GetOrCreateSfxSource()
    {
        foreach (var src in _sfxPool)
            if (!src.isPlaying)
                return src;

        var inst = Instantiate(_sfxSourcePrefab, transform);
        _sfxPool.Add(inst);
        return inst;
    }

    private IEnumerator DecrementSfxCountAfter(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playingSfxCount.TryGetValue(clip, out int cnt) && cnt > 0)
            _playingSfxCount[clip] = cnt - 1;
    }
    //──────────────────────────────────────────────
    // BGM 재생 (플레이리스트 시작 인덱스 지정 가능)

    /// <summary>
    /// BGM 플레이리스트 재생을 시작합니다.
    /// startIndex: 재생할 트랙의 시작 인덱스 (0-based)
    /// loopPlaylist: 플레이리스트 끝나면 반복 재생 여부
    /// </summary>
    public void PlayBgmList(int startIndex, bool loopPlaylist = false)
    {
        if (_bgmPlaylist == null || _bgmPlaylist.Count == 0)
            return;

        
        _bgmIndex = Mathf.Clamp(startIndex, 0, _bgmPlaylist.Count - 1);
        _bgmLoop  = loopPlaylist;

        // 기존 재생 중단
        StopBgm();

        // 새 코루틴 시작
        _bgmCoroutine = StartCoroutine(BgmPlaybackRoutine());
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 즉시 멈춥니다.
    /// </summary>
    public void StopBgm()
    {
        if (_bgmCoroutine != null)
        {
            StopCoroutine(_bgmCoroutine);
            _bgmCoroutine = null;
        }
        if (_bgmSource.isPlaying)
            _bgmSource.Stop();
    }

    private IEnumerator BgmPlaybackRoutine()
    {
        while (_bgmPlaylist != null && _bgmPlaylist.Count > 0)
        {
            var clip = _bgmPlaylist[_bgmIndex];
            _bgmSource.clip = clip;
            _bgmSource.Play();

            // 곡 길이만큼 대기
            yield return new WaitForSeconds(clip.length);

            // 다음 곡으로
            _bgmIndex++;
            if (_bgmIndex >= _bgmPlaylist.Count)
            {
                if (_bgmLoop)
                    _bgmIndex = 0;
                else
                    break;
            }
        }

        _bgmCoroutine = null;
    }
    public void OnSfxVolumeChanged(float value)
    {
        Debug.Log("[SoundManager] SFX Volume → " + value);
        // 프리팹 기본 볼륨 설정
        _sfxSourcePrefab.volume = value;
        _sfxVolume = value;

        // 이미 풀에 생성된 모든 SFX AudioSource에도 적용
        foreach (var src in _sfxPool)
        {
            src.volume = value;
        }
    }
    public void OnBgmVolumeChanged(float value)
    {
        _bgmVolume = value;
        Debug.Log("[SoundManager] BGM Volume → " + value);
        if (_bgmSource != null)
        {
            _bgmSource.volume = value;
        }
    }
}
