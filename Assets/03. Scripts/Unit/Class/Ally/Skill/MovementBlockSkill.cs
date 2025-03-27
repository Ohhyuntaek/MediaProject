using UnityEngine;

public class MovementBlockSkill : ISkill
{
    
    public void Activate(MonoBehaviour caster)
    {
        if (CheckAlllyOrPlayer(caster))
        {
            Debug.Log($"[{caster.name}] 이동 제한 스킬 발동!");
            //적 범위 확인 후 스킬 효과 구현 필요 => 맵이 어느정도 완성되면  구현 예정 
        }
        
        
    }

    private bool CheckAlllyOrPlayer(MonoBehaviour caster)
    {
        return caster is Ally ally || caster is Player;
    }
}
