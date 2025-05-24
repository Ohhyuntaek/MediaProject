using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private List<ItemData> selectedItems;

    public List<ItemData> GetSelectedItem() => selectedItems;
    
    public void SelectItem(ItemData item)
    {
        selectedItems.Add(item);
    }
}
