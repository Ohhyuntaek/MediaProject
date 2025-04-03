using UnityEngine;
using System.Collections;

public class AllyDebuffAttackState : IState<Ally>
{
    private string _unitName;
    private bool finished = false;
    private int rand;
    public void Enter(Ally ally)
    {
        _unitName = ally.UnitData.UnitName;
        rand = Random.Range(0, 3);
        PlaySlamanderDebuffAnimation(ally);
        //ally.StartCoroutine(DebuffRoutine(ally));
    }

    
   
    private void  PlaySlamanderDebuffAnimation(Ally ally)
    {
        string trigger = GetTriggerByIndex(rand);
        Debug.Log($"{ally.UnitData.UnitName} 이 {trigger} 스킬 시전");
        
        ally.Animator.SetTrigger(trigger);
        Debug.Log("스킬 시전");
        
    }

    
    

    private string GetTriggerByIndex(int index)
    {
        return index switch
        {
            0 => "3_Fire",
            1 => "3_Thunder",
            2 => "3_Ice",
            _ => "3_Fire"
        };
    }

    public void Update(Ally ally)
    {
        TranstionTo(ally);
    }

    private void TranstionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if ((stateInfo.IsName("Debuff") || stateInfo.IsName("Debuff1") ||stateInfo.IsName("Debuff2"))  && !finished && stateInfo.normalizedTime > 0.9f)
        {
            finished = true;
            ally.ChangeState(new AllyIdleState());
            GameObject skillEffectPrefab = ally.UnitData.SkillEffect[rand];

        
            Vector3 spawnPos = ally.transform.position + Vector3.up * 1.5f; //TODO : 적군 위치에 맞게 스폰시키기 
            Quaternion spawnRot = Quaternion.identity;

            if (skillEffectPrefab != null)
            {
                GameObject effect = Object.Instantiate(skillEffectPrefab, spawnPos, spawnRot);
                Object.Destroy(effect, 2f); 
            }
        }
    }

    public void Exit(Ally ally) { }
}