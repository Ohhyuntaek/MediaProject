using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefab;  // 아이템 슬롯 프리팹
    [SerializeField] private Transform contentParent;     // ScrollView의 Content

    public void RefreshUI(List<ItemEffectBase> items)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, contentParent);
            ItemSlotUI slot = slotGO.GetComponent<ItemSlotUI>();
            slot.Setup(item);
        }
    }
    
}
