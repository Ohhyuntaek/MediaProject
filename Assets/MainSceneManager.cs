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

    [Header("Dawn 선택")]
    private DawnData selectedDawnData;
    private DawnSelector currentSelectedButton; // 현재 선택된 버튼 기억

    void Awake()
    {
        Instance = this;
        startButton.interactable = false; // Dawn 선택 전까지 비활성화
    }

    /// <summary>
    /// 버튼이 호출하는 함수
    /// </summary>
    public void OnSelectDawn(DawnData dawnData, DawnSelector button)
    {
        selectedDawnData = dawnData;
        UpdateDawnUI();

        // 버튼 테두리 갱신
        if (currentSelectedButton != null)
            currentSelectedButton.SetHighlight(false);

        currentSelectedButton = button;
        currentSelectedButton.SetHighlight(true);

        startButton.interactable = true;
    }

    private void UpdateDawnUI()
    {
        if (dawnPortraitImage != null)
            dawnPortraitImage.sprite = selectedDawnData.Portrait;

        if (dawnHpSlider != null)
        {
            dawnHpSlider.maxValue = hpMaxValue;
            dawnHpSlider.value = selectedDawnData.MaxHP;
        }

        if (dawnEnergySlider != null)
        {
            dawnEnergySlider.maxValue = energyMaxValue;
            dawnEnergySlider.value = selectedDawnData.InitialEnergy;
        }
    }

    public void OnStartButtonClick()
    {
        if (selectedDawnData == null)
        {
            Debug.LogWarning("Dawn을 선택하지 않았습니다!");
            return;
        }

        GameManager.Instance.SetSelectedDawn(selectedDawnData);
        SceneManager.LoadScene("InStage");
    }
}