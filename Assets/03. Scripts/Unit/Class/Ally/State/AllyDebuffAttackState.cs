using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllyDebuffAttackState : IState<Ally>
{
    private string _unitName;
    private bool finished = false;
    private int index;
    public void Enter(Ally ally)
    {
        _unitName = ally.UnitData.UnitName;
         ally.SetSkillRandomNum(Random.Range(0, 3));
         index = ally.GetSkillRandomNum();
        PlaySlamanderDebuffAnimation(ally);
        SoundManager.Instance.PlaySfx(ally.UnitData.SkillSound[index],ally.transform.position);
        //ally.StartCoroutine(DebuffRoutine(ally));
    }

    
   
    private void  PlaySlamanderDebuffAnimation(Ally ally)
    {
        string trigger = GetTriggerByIndex(index);
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
            SpawnEffect(ally);
            ally.PerformSkill();
            ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
          
        }
    }

    private void SpawnEffect(Ally ally)
    {   
        
        GameObject skillEffectPrefab = ally.UnitData.SkillEffect[index];
        List<IDamageable> _detectList = ally.DetectNearestTileTargets();
        if (_detectList.Count > 0)
        {
            var _detectEnemy = _detectList[0] as MonoBehaviour;
            Vector3 spawnPos = _detectEnemy.transform.position;
            Quaternion spawnRot = Quaternion.identity;
            if (skillEffectPrefab != null)
            {
                GameObject effect = Object.Instantiate(skillEffectPrefab, spawnPos, spawnRot);
                Object.Destroy(effect, 2f);
            }
        }
    }

    public void Exit(Ally ally)
    {
        ally.SetFinalSkill(true);
    }
}