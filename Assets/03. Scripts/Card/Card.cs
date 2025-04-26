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
    public UnitData unitData;
    public AllyType allyType;
    public LineType lineType;
    
    public void OnButtonClick()
    {
        // UnitData를 allyType으로 가져오기
        UnitData unitData = InStageManager.Instance.GetUnitDataByAllyType(allyType);

        if (unitData == null)
        {
            Debug.LogError($"Card에 대한 UnitData를 찾을 수 없습니다. AllyType: {allyType}");
            return;
        }

        if (InStageManager.Instance.GetCost() < unitData.Cost)
        {
            Debug.Log("코스트가 부족합니다! 소환할 수 없습니다.");
            return;
        }
        
        GameObject ally = AllyPoolManager.Instance.SpawnAlly(allyType, lineType);

        if (ally != null)
        {
            Destroy(this.gameObject);
            InStageManager.Instance.ShiftCardsLeft(slotIndex);   
        }
        else
        {
            Debug.Log("No available tile or ally in pool");
        }
    }
}
