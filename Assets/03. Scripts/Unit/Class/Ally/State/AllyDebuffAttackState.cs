using UnityEngine;
using System.Collections;

public class AllyDebuffAttackState : IState<Ally>
{
    private string _unitName;
    private bool finished = false;
    public void Enter(Ally ally)
    {
        _unitName = ally.UnitData.UnitName;
        PlaySlamanderDebuffAnimation(ally);
        //ally.StartCoroutine(DebuffRoutine(ally));
    }

    private IEnumerator DebuffRoutine(Ally ally)
    {
        switch (ally.UnitData.UnitName)
        {
            case "Slamander":
                PlaySlamanderDebuffAnimation(ally);
                break;

            case "DebuffCaster":
                yield return PlayDebuffCasterAnimation(ally);
                break;

            default:
                Debug.LogWarning($"[AllyDebuffAttackState] {ally.UnitData.UnitName}의 디버프 FSM 없음. Idle로 전환");
                break;
        }

        ally.ChangeState(new AllyIdleState());
    }

   
    private void  PlaySlamanderDebuffAnimation(Ally ally)
    {
        int rand = Random.Range(0, 3);
        string trigger = GetTriggerByIndex(rand);
        Debug.Log($"{ally.UnitData.UnitName} 이 {trigger} 스킬 시전");
        
        ally.Animator.SetTrigger(trigger);

        
        GameObject skillEffectPrefab = ally.UnitData.SkillEffect[rand];

        
        Vector3 spawnPos = ally.transform.position + Vector3.up * 1.5f; //TODO : 적군 위치에 맞게 스폰시키기 
        Quaternion spawnRot = Quaternion.identity;

        if (skillEffectPrefab != null)
        {
            GameObject effect = Object.Instantiate(skillEffectPrefab, spawnPos, spawnRot);
            Object.Destroy(effect, 2f); 
        }
        
        ally.PerformAttack();
    }

    
    private IEnumerator PlayDebuffCasterAnimation(Ally ally)
    {
        int rand = Random.Range(0, 3);
        string trigger = GetTriggerByIndex(rand);

        ally.Animator.SetTrigger(trigger);
        yield return new WaitForSeconds(1f / ally.UnitData.AttackSpeed);

        ally.PerformAttack();
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
        }
    }

    public void Exit(Ally ally) { }
}