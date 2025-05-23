using Sirenix.OdinInspector;
using UnityEngine;

public enum ItemType { Ally, Remnant }

[CreateAssetMenu(fileName = "NewItemData", menuName = "SO/Item Data")]

public class ItemData : ScriptableObject
{
    [Title("아이템 정보")]
    [SerializeField, LabelText("아이템 이름")] private string _itemName;
    [SerializeField, LabelText("아이템 종류")] private ItemType _itemType; 
    [SerializeField, LabelText("아이템 설명")] private string _description;
    [SerializeField, LabelText("아이템 이미지")] private Sprite _itemImage;
    
    public string ItemName => _itemName;
    public ItemType ItemType => _itemType;
    public string Description => _description;
    public Sprite ItemImage => _itemImage;
}
