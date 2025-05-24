using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance;

    [Header("UI 연결")]
    [SerializeField] private int hpMaxValue;
    [SerializeField] private int energyMaxValue;
    [SerializeField] private Image dawnPortraitImage;
    [SerializeField] private Slider dawnHpSlider;
    [SerializeField] private Slider dawnEnergySlider;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text passiveSkillInfoText;
    [SerializeField] private TMP_Text aciveSkillInfoText;

    [Header("Book Manager 연결")]
    [SerializeField] private BookManager bookManager;
    
    
    private Dawn selectedDawn;
    private DawnSelector currentSelectedButton; // 현재 선택된 버튼 기억

    void Awake()
    {
        Instance = this;
        startButton.interactable = false; // Dawn 선택 전까지 비활성화
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            EntireGameManager.Instance.soundManager.PlayBgmList(0,true);
        }
     
    }

    /// <summary>
    /// 버튼이 호출하는 함수
    /// </summary>
    public void OnSelectDawn(Dawn dawn, DawnSelector button)
    {
        selectedDawn = dawn;
        UpdateDawnUI();

        // 버튼 테두리 갱신
        if (currentSelectedButton != null)
            currentSelectedButton.SetHighlight(false);

        passiveSkillInfoText.text = dawn.DawnData.PassiveSkillInfo;
        aciveSkillInfoText.text = dawn.DawnData.AciveSkillInfo;
        
        currentSelectedButton = button;
        currentSelectedButton.SetHighlight(true);

        startButton.interactable = true;
    }

    private void UpdateDawnUI()
    {
        if (dawnPortraitImage != null)
            dawnPortraitImage.sprite = selectedDawn.DawnData.Portrait;

        if (dawnHpSlider != null)
        {
            dawnHpSlider.maxValue = hpMaxValue;
            dawnHpSlider.value = selectedDawn.DawnData.MaxHP;
        }

        if (dawnEnergySlider != null)
        {
            dawnEnergySlider.maxValue = energyMaxValue;
            dawnEnergySlider.value = selectedDawn.DawnData.InitialEnergy;
        }
    }

    public void OnStartButtonClick()
    {
    
        if (selectedDawn == null)
        {
            Debug.LogWarning("Dawn을 선택하지 않았습니다!");
            return;
        }

        RuntimeDataManager.Instance.SetSelectedDawn(selectedDawn);
        bookManager.CloseBook();
        SceneManager.LoadScene("MapScene");
        
    }
}