using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AllyAttackState : IState<Ally>
{
    private bool finished = false;
    private bool firstAttack = false;
    private List<Enemy> _detected;
    private bool dir = false;
    public void Enter(Ally ally)
    {
        dir = ally.Dircetion;
        ally.Animator.SetTrigger("2_Attack");
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
                    _detected = ally.DetectTargets(2);
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
                    _detected = ally.DetectNearestEnemyTileEnemies();
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
                    //if(발판을 밝고 있을 시))//TODO : 범위공격
                    //else(발판 미적용 시) //TODO : 단일공격
                    //TODO : 바운티 헌터 구현 
                    break;
                case "NightLord" :
                    ally.ChangeState(new AllyIdleState(1/ally.ATKSPD));
                    break;
                case "CentaurLady" :
                    _detected = ally.DetectNearestEnemyTileEnemies();
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