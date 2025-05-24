using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    
    private List<ItemEffectBase> selectedItems = new List<ItemEffectBase>();
    private List<RemnantSO> selectedRemnant = new List<RemnantSO>();
    private List<AllyItemData> selectedAllyItem = new List<AllyItemData>();

    /// <summary>
    /// 수집된 아이템 목록을 반환합니다.
    /// </summary>
    public List<ItemEffectBase> GetSelectedItems() => selectedItems;
    
    /// <summary>
    /// 수집된 RemnantSO 목록을 반환합니다.
    /// </summary>
    public List<RemnantSO> GetSelectedRemnants() => selectedRemnant;

    /// <summary>
    /// 수집된 AllyItemData 목록을 반환합니다.
    /// </summary>
    public List<AllyItemData> GetSelectedAllyItems() => selectedAllyItem;
    /// <summary>
    /// 아이템을 수집 목록에 추가합니다.
    /// </summary>
    public void SelectItem(ItemEffectBase item)
    {
        if (item == null) return;
        selectedItems.Add(item);
        switch (item)
        {
            case RemnantSO remnant:
                selectedRemnant.Add(remnant);
                Debug.Log("우닛 능력치 증가 아이템 획득");
                break;

            case AllyItemData allyItem:
                selectedAllyItem.Add(allyItem);
                Debug.Log("유닛 확률 증가 아이템");
                break;
        }
    }
    public void ClearAll()
    {
        selectedItems.Clear();
        selectedRemnant.Clear();
        selectedAllyItem.Clear();
    }

    
}