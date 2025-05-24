using UnityEngine;

public abstract class ItemEffectBase : ScriptableObject
{
    [SerializeField, Tooltip("아이템 아이콘")]
    private Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField, Tooltip("아이템 가격")]
    private int _price;
    public int Price => _price;

    [SerializeField, Tooltip("설명 텍스트")]
    private string _description;
    public string Description => _description;

    public abstract void Apply();


}