using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AllyIdleState : IState<Ally>
{
    private float _attackTimer;
    private List<Enemy> _detected;
    private string unitName;
    
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
        if (_attackTimer <= 0f)
        {
            switch (ally.UnitData.UnitName)
            {
                case "Slamander":
                    if (ally.DetectTargets(ally.UnitData.AttackRange).Count > 0)
                    {
                        ally.ChangeState(new AllyAttackState());
                    }
                    else if(ally.DetectNearestEnemyTileEnemies().Count>0)
                    {
                        ally.ChangeState(new AllyDebuffAttackState());
                    }
                    else
                    {
                        ally.ChangeState(new AllyIdleState(_attackTimer));
                    }
                    break;
                default:
                    if (ally.DetectTargets(ally.UnitData.AttackRange).Count > 0)
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