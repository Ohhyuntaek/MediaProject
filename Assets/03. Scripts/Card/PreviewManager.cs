using System;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PreviewManager : MonoBehaviour
{
    [Header("UnitData 리스트")]
    [SerializeField] private List<UnitData> _unitDataList;
    [Header("프리뷰 모델 리스트 (UnitData 순서와 1:1 매핑)")]
    [SerializeField] private List<GameObject> _previewModel;
    [Header("테스트용 타일")]
    [SerializeField] private AllyTile _testTile;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private GameObject tooltipCard;
    // 풀링된 프리뷰 모델 & 현재 어떤 프리팹을 띄웠는지 추적
    private GameObject _pooledPreviewModel;
    private GameObject _currentModelPrefab;



   [SerializeField] private TextMeshProUGUI _attackText;
   [SerializeField]  private TextMeshProUGUI _durationText;
   [SerializeField] private TextMeshProUGUI _recomendText;
   [SerializeField] private TextMeshProUGUI _unitNameText;
   [SerializeField] private TextMeshProUGUI _nomalText;
   [SerializeField] private TextMeshProUGUI _skillText;
   [SerializeField] private TextMeshProUGUI _typeText;
   [SerializeField] private Image _unitPortrait;
    
    
    private List<GameObject> _selectedRange = new List<GameObject>();
    private List<SpriteRenderer> _selectedRenderers = new List<SpriteRenderer>();

    private bool _onPreview = false;

    private void Start()
    {
        
        if (tooltipCard != null)
        {
            tooltipCard.SetActive(false);
            // _attackText = GameObject.FindGameObjectWithTag("AttackText").GetComponent<TextMeshProUGUI>();
            // _durationText = GameObject.FindGameObjectWithTag("DurationText").GetComponent<TextMeshProUGUI>();
            // _recomendText = GameObject.FindGameObjectWithTag("RecomedText").GetComponent<TextMeshProUGUI>();
            // _unitNameText = GameObject.FindGameObjectWithTag("UnitNameText").GetComponent<TextMeshProUGUI>();
            // _nomalText = GameObject.FindGameObjectWithTag("NomalText").GetComponent<TextMeshProUGUI>();
            // _skillText = GameObject.FindGameObjectWithTag("UnitNameText").GetComponent<TextMeshProUGUI>();
            // _typeText = GameObject.FindGameObjectWithTag("TypeText").GetComponent<TextMeshProUGUI>();
            // _unitPortrait =  GameObject.FindGameObjectWithTag("Portrait").GetComponent<Image>();


        }
    }

    public void ShowTooltip(UnitData unitData)
    {
        _attackText.text = "공격력 : " + unitData.BaseAttack;
        _durationText.text = "지속시간 : " + (int)unitData.Duration;
        _typeText.text = "선호 라인 : " + unitData.UnitType.ToString();
        _unitNameText.text = GetUnitNmae(unitData.AllyType);
        _skillText.text = "스킬 : "+unitData.SkillDescriptor;
        _unitPortrait.sprite = unitData.Sprite;
        _nomalText.text = "일반 공격 : " + unitData.NomalDescriptor;
        _recomendText.text = unitData.RecomendDescriptor;
        tooltipCard.SetActive(true);

    }

    public void ClearToolTip()
    {
        tooltipCard.SetActive(false);
    }

    private string GetUnitNmae(AllyType ally)
    {
        switch (ally)
        {
            case AllyType.JoanDarc:
                return "잔다르크";
            case AllyType.Rogue:
                return "로그";
            case AllyType.Salamander:
                return "샐러멘더";
            case AllyType.CentaurLady:
                return "켄타로사우스";
            case AllyType.BountyHunter:
                return "바운티 헌터";
            case AllyType.NightLord:
                return "나이트로드";
            default:
                return "오류";
            
        }
    }


    public void ShowPreview(UnitData unitData)
    {
        ClearPreview();
        _onPreview = true;
        AllyTile tile = _tileManager.GetPreviewedTile();
        
        int idx = _unitDataList.FindIndex(u => u.AllyType == unitData.AllyType);
        if (idx < 0 || idx >= _previewModel.Count) return;
        var prefab = _previewModel[idx];

        // 2) 모델 풀링 재생성 로직
        if (_pooledPreviewModel == null || _currentModelPrefab != prefab)
        {
            // 이전 모델이 있으면 파괴
            if (_pooledPreviewModel != null)
                Destroy(_pooledPreviewModel);

            // 새로 생성
            _pooledPreviewModel    = Instantiate(prefab, tile.transform);
            _pooledPreviewModel.transform.localPosition = Vector3.zero;
            _currentModelPrefab    = prefab;
        }
        else
        {
            // 동일 프리팹이면 위치만 갱신
            _pooledPreviewModel.transform.SetParent(tile.transform, false);
            _pooledPreviewModel.transform.localPosition = Vector3.zero;
            _pooledPreviewModel.SetActive(true);
        }

        // 캐릭터가 바라보는 방향 반전
        var sr = _pooledPreviewModel.GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = tile.dir;


        _selectedRange = GridTargetManager.Instance.GetPatternGameObjectsFast(unitData.DetectionPatternSo, tile);
        _selectedRenderers.Clear();
        foreach (var go in _selectedRange)
        {
            var r = go.GetComponent<SpriteRenderer>();
            if (r != null)
            {
                r.enabled = true;
                _selectedRenderers.Add(r);
            }
        }
    }

    // 프리뷰 해제
    public void ClearPreview()
    {
        if (_pooledPreviewModel != null)
            _pooledPreviewModel.SetActive(false);

        foreach (var r in _selectedRenderers)
            r.enabled = false;

        _selectedRange.Clear();
        _selectedRenderers.Clear();
        _onPreview = false;
    }
}
