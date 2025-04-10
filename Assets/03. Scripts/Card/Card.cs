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

    public AllyType allyType;
    public LineType lineType;
    
    public void OnButtonClick()
    {
        GameObject ally = AllyPoolManager.Instance.SpawnAlly(allyType, lineType);

        if (ally != null)
        {
            Destroy(this.gameObject);
            GameManager.Instance.ShiftCardsLeft(slotIndex);   
        }
        else
        {
            Debug.Log("No available tile or ally in pool");
        }
    }
}
