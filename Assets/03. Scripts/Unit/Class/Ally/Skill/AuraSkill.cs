
using UnityEngine;

public class AuraSkill : ISkill<Ally>
{
    public void Activate(Ally owner)
    {
        AllyTile _moveableTile = InGameSceneManager.Instance.tileManager.MoveableTile();
        if (_moveableTile != null)
        {
            InGameSceneManager.Instance.tileManager.FreeTile(owner.transform.position);
            owner.transform.position = _moveableTile.transform.position;
            owner.SetOccupiedTile(_moveableTile);

        }
        
       
        
    }
}
