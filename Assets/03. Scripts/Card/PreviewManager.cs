using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    public static PreviewManager Instance { get; private set; }

    [Header("UnitData 리스트 (스크립터블 객체)")]
    [SerializeField] private List<UnitData> _unitDataList;

    [Header("미리보기 모델 리스트 (UnitData 순서와 1:1 매핑)")]
    [SerializeField] private List<GameObject> _previewModel;

    [Header("테스트용 타일 (Inspector에 드래그)")]
    [SerializeField] private AllyTile _testTile;

    // 현재 선택된 타일·범위·모델
    private AllyTile _selectedTile;
    private List<GameObject> _selectedRange;
    private GameObject _selectedModel;

    // 인스턴스화된 프리뷰 모델
    private GameObject _spawnedModel;

    // 프리뷰 모드 상태 플래그
    private bool _onPreview = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _selectedRange = new List<GameObject>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // 기존 ShowPreview API 유지 (AllyTile 직접 넘길 때)
    public void ShowPreview(AllyTile tile, UnitData unitData)
    {
        // 이전 프리뷰 정리
        ClearPreview();

        // 셋업
        _selectedTile  = tile;
        _selectedModel = GetModelByType(unitData.AllyType);
        _selectedRange = GridTargetManager.Instance
                            .GetPatternGameObjects(unitData.DetectionPatternSo, tile);

        // 프리뷰 모드 on
        _onPreview = true;

        // 모델 스폰
        if (_selectedModel != null)
        {
            var sr = _selectedModel.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = tile.dir;

            _spawnedModel = Instantiate(
                _selectedModel,
                tile.transform.position,
                Quaternion.identity
            );
        }

        // 타일 하이라이트
        foreach (var t in _selectedRange)
            t.GetComponent<SpriteRenderer>().enabled = true;
    }

    // **테스트용**: 내부에 지정한 _testTile 만 사용
    public void ShowPreview(UnitData unitData)
    {
        if (_testTile == null)
        {
            Debug.LogWarning("[PreviewManager] _testTile이 할당되지 않았습니다.");
            return;
        }
        ShowPreview(_testTile, unitData);
    }

    /// <summary>현재 프리뷰 전부 초기화</summary>
    public void ClearPreview()
    {
        // 모델 제거
        if (_spawnedModel != null)
        {
            Destroy(_spawnedModel);
            _spawnedModel = null;
        }
        // 타일 하이라이트 해제
        if (_selectedRange != null)
        {
            foreach (var t in _selectedRange)
                t.GetComponent<SpriteRenderer>().enabled = false;
            _selectedRange.Clear();
        }
        // 상태 리셋
        _selectedTile  = null;
        _selectedModel = null;
        _onPreview     = false;
    }

    // ---- 내부 헬퍼 ----
    private GameObject GetModelByType(AllyType type)
    {
        for (int i = 0; i < _unitDataList.Count; i++)
            if (_unitDataList[i].AllyType == type)
                if (i < _previewModel.Count)
                    return _previewModel[i];
        Debug.LogWarning($"[PreviewManager] 모델을 찾을 수 없습니다: {type}");
        return null;
    }
}
