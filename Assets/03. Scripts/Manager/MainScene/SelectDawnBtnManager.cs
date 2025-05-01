using UnityEngine;
using UnityEngine.UI;

public class SelectDawnBtnManager : MonoBehaviour
{
    [SerializeField] private Dawn dawn;

    [SerializeField] private int hpMaxValue;
    [SerializeField] private int energyMaxValue;
    // Dawn UI 갱신용
    [SerializeField] private Image dawnImage;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider energySlider;
    
    public void OnSelect()
    {
        // 1. GameManager에 저장
        GameManager.Instance.SetSelectedDawn(dawn);

        // 2. UI 갱신
        if (dawnImage != null)
            dawnImage.sprite = dawn.DawnData.Portrait;

        if (hpSlider != null)
        {
            hpSlider.maxValue = hpMaxValue;
            hpSlider.value = dawn.DawnData.MaxHP;
        }

        if (energySlider != null)
        {
            energySlider.maxValue = energyMaxValue;
            energySlider.value = dawn.DawnData.MaxEnergy;
        }
    }
}
