using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public enum CardType
{
    FrontLine,
    MidLine,
    RearLine
}

public class Card : MonoBehaviour
{
    public int slotIndex;

    public CardType cardType;
    
    public void OnButtonClick()
    {
        Destroy(this.gameObject);
        
        GameManager.Instance.ShiftCardsLeft(slotIndex);
    }
}
