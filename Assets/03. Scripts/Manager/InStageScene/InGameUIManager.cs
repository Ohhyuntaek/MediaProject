using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;
    
    [Header("UI")]
    [SerializeField, LabelText("현재 스테이지 텍스트")] private TMP_Text stageText;
    [SerializeField, LabelText("선택한 Dawn의 초상화")] private Image dawnImage;
    [SerializeField, LabelText("선택한 Dawn의 HP")] private Slider hpSlider;
    [SerializeField, LabelText("선택한 Dawn의 Energy")] private Slider energySlider;
    [SerializeField, LabelText("선택한 Dawn의 액티브 스킬 쿨다운 프로그레스")] 
    private DawnCoolTimeProgress dawnCoolTimeProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // UI 초기화
        UpdateDawnImage(GameManager.Instance.GetSelectedDawn().DawnData);
        SetDawnCoolTimeProgress(InGameSceneManager.Instance.dawnSpawnManager.spawnedDawn);

    }

    /// <summary>
    /// 스테이지 이름 설정
    /// </summary>
    /// <param name="stageName"></param>
    public void SetStageText(string stageName)
    {
        stageText.text = stageName;
    }
    
    /// <summary>
    /// Dawn Profile의 이미지를, 선택한 Dawn 캐릭터 이미지로 업데이트
    /// </summary>
    /// <param name="dawn"></param>
    public void UpdateDawnImage(DawnData dawn)
    {
        dawnImage.sprite = dawn.Portrait;
    }

    /// <summary>
    /// Dawn Profile의 HP Slider를, 선택한 Dawn 캐릭터의 HP로 업데이트
    /// </summary>
    public void UpdateHpSlider(Dawn dawn)
    {
        Debug.Log($"{dawn.DawnData.MaxHP.ToString()}, {dawn.CurrentHP.ToString()}");
        hpSlider.value = dawn.DawnData.MaxHP > 0 ? dawn.CurrentHP / dawn.DawnData.MaxHP : 0;
    }

    /// <summary>
    /// Dawn Profile의 Energy Slider를, 선택한 Dawn 캐릭터의 Energy로 업데이트
    /// </summary>
    /// <param name="dawn"></param>
    public void UpdateEnergySlider(Dawn dawn)
    {
        energySlider.value = dawn.DawnData.MaxEnergy > 0 ? dawn.currentEnergy / dawn.DawnData.MaxEnergy : 0;
    }
    
    /// <summary>
    /// 쿨타임 원형 프로그레스의 타겟을, 소환된 dawn으로 설정 
    /// </summary>
    /// <param name="dawn"></param>
    public void SetDawnCoolTimeProgress(Dawn dawn)
    {
        dawnCoolTimeProgress.SetTargetDawn(dawn);
    }
}
