using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    // [SerializeField] private TMP_Text descriptionText;

    /// <summary>
    /// 슬롯에 아이템 정보를 표시
    /// </summary>
    public void Setup(ItemEffectBase item)
    {
        if (item == null) return;

        if (iconImage != null) iconImage.sprite = item.Icon;
        if (priceText != null) priceText.text = $"{item.Price}G";
        // if (descriptionText != null) descriptionText.text = item.Description;
    }
}