using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AllyIdleState : IState<Ally>
{
    private float _attackTimer;
    private List<IDamageable> _detected;
    private string unitName;
    private float _waitTimer; 
    
    public AllyIdleState(float timer)
    {   
        _attackTimer = timer;
    }
    
    public void Enter(Ally ally)
    {
        unitName = ally.UnitData.UnitName;
        ally.Animator?.SetBool("1_Move", false);
        
    }

    public void Update(Ally ally)
    {
        //Debug.Log("check");
        _attackTimer -= Time.deltaTime;
        _waitTimer += Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            switch (ally.UnitData.UnitName)
            {
                case "Slamander":
                    if (ally.DetectTargets().Count > 0)
                    {
                        ally.ChangeState(new AllyAttackState());
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(_attackTimer));
                    }
                    break;
                case "CentaurLady":
                    if (ally.DetectTargets().Count > 0)
                    {
                        ally.ChangeState(new AllyAttackState());
                    }
                    else if(_waitTimer>3f && ally.DetectNearestTileTargets().Count>0 && !ally.FinalSkill)
                    {
                        ally.ChangeState(new AllySpecialAttackState(ally.Dircetion));
                    }
                    break;
                case "BountyHunter":
                    _detected = ally.DetectTargets();
                    if (_detected.Count > 0)
                    {
                        if (ally.OnTile)
                        {
                            Transform transform = (_detected[0] as MonoBehaviour)?.transform;
                            ally.ChangeState(new BountyHunterSpecialAttackState(transform));
                        }
                        else
                        {
                            ally.ChangeState(new AllyAttackState());
                        }
                    }
                    break;
                case "Rogue":
                    _detected = ally.DetectTargets();
                    if (_detected.Count > 0)
                    {
                        if (ally.OnTile)
                        {
                            ally.ChangeState(new RogueSpecialAttackState());
                        }
                        else
                        {
                            ally.ChangeState(new AllyAttackState());
                        }
                    }
                    break;
                case "Killren":
                    _detected = ally.DetectTargets();
                    if (_detected.Count > 0)
                    {
                        bool ontile1 = ally.Dircetion && ally.OccupiedTile.LineType == LineType.Front;
                        bool ontile2 = ally.Dircetion == false && ally.OccupiedTile.LineType == LineType.Rear;
                        if (ontile1 || ontile2)
                        {
                            ally.ChangeState(new KillrenSpecialAttackState());
                        }
                        else
                        {
                            ally.ChangeState(new AllyAttackState());
                        }
                    }
                    break;
                case "Aura":
                    int lifetime = (int) ally.GetTotalLifeTime;
                    if (lifetime % 5 == 0)
                    {
                        ally.ChangeState(new AuraSpecialAttackState());
                    }
                    else
                    {
                        ally.ChangeState(new AllyAttackState());
                    }

                    break;
                case "Diabunny":
                    int randomNum = Random.Range(1, 11);
                    bool isSpecial = randomNum > 3;
                    if (isSpecial)
                    {
                        //특수 공격
                    }
                    else
                    {
                        //일반 공격 
                    }

                    break;
                    
                
                    
                default:
                    if (ally.DetectTargets().Count > 0)
                    {
                        ally.ChangeState(new AllyAttackState());
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(_attackTimer));
                    }
                    break;
            }
           
        }
        
        
       
    }

    public void Exit(Ally ally) { }
}