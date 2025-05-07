using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AllyAttackState : IState<Ally>
{
    private bool finished = false;
    private bool firstAttack = false;
    private List<IDamageable> _detected;
    private bool dir = false;
    public void Enter(Ally ally)
    {
        dir = ally.Dircetion;
        ally.Animator.SetTrigger("2_Attack");
        SoundManager.Instance.PlaySfx(ally.UnitData.AttackSound[0],ally.transform.position);
    }
    

    public void Update(Ally ally)
    {
        TranstionTo(ally);
    }

    private void TranstionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && !firstAttack && stateInfo.normalizedTime > 0.5f && ally.UnitData.UnitName =="CentaurLady")
        {
            firstAttack = true;
            ally.PerformAttack();
        }
        if (stateInfo.IsName("Attack") && !finished && stateInfo.normalizedTime > 0.9f)
        {
            finished = true;
            ally.PerformAttack();
            
            switch (ally.UnitData.UnitName)
            {
                case "Jandark":
                    _detected = ally.DetectTargets();
                    if (_detected.Count > 0)
                    {
                        ally.ChangeState(new KnockbackAttackState());
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    }
                    break;

                case "Slamander":
                    _detected = ally.DetectNearestTileTargets();
                    if (!ally.FinalSkill && _detected.Count>0)
                    {
                    
                        ally.ChangeState(new AllyDebuffAttackState());
                        
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    }
                    break;
                case "BountyHunter" :
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
                case "NightLord" :
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
                case "CentaurLady" :
                    _detected = ally.DetectNearestTileTargets();
                    if (!ally.FinalSkill && _detected.Count>0)
                    {
                        
                        ally.SetFinalSkill(true);
                        ally.ChangeState(new AllySpecialAttackState(dir));
                        
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    }
                    break;

                default:
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
            }
        }
    }

    public void Exit(Ally ally)
    {
        
        
    }
}