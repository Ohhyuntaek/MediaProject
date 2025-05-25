using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("강화 효과 텍스트")] 
    [SerializeField] private TMP_Text costMultiplierText;
    [SerializeField] private TMP_Text cardSpawnIntervalMultiplierText;
    [SerializeField] private TMP_Text cooldownMultiplierText;
    [SerializeField] private TMP_Text energyChargeMultiplierText;

    [Header("루멘 텍스트")] 
    [SerializeField] private TMP_Text lumenText;
    
    public ItemInventoryUI inventoryUI;
    
    // [Header("보유 아이템")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        costMultiplierText.text = RuntimeDataManager.Instance.enhancement.costMultiplier.ToString("F1");
        cardSpawnIntervalMultiplierText.text = RuntimeDataManager.Instance.enhancement.cardSpawnIntervalMultiplier.ToString("F1");
        cooldownMultiplierText.text = RuntimeDataManager.Instance.enhancement.cooldownMultiplier.ToString("F1");
        energyChargeMultiplierText.text = RuntimeDataManager.Instance.enhancement.energyChargeMultiplier.ToString("F1");
        
        lumenText.text = RuntimeDataManager.Instance.lumenCalculator.Lumen.ToString();
        
        inventoryUI.RefreshUI(RuntimeDataManager.Instance.itemCollector.GetSelectedItems());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
