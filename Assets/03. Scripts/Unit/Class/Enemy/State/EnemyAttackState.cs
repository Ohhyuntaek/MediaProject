using UnityEngine;

public class EnemyAttackState : IState<Enemy>
{
    

    public void Enter(Enemy enemy)
    {
        enemy.Animator?.SetTrigger("2_Attack");
     
    }

    public void Update(Enemy enemy)
    {
        AnimatorStateInfo stateInfo = enemy.Animator.GetCurrentAnimatorStateInfo(0);
        if (enemy.IsTargetInRange())
        {
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime > 0.9f)
            {
                Debug.Log("헤이맨");
                switch (enemy.EnemyData.EnemyName)
                {
                    case "Basic3" :
                        Debug.Log("스킬팡팡");
                        enemy.PerformSkill();
                        enemy.ChangeState(new EnemyIdleState());
                        break;
                    default: 
                        enemy.PerformAttack();
                        enemy.ChangeState(new EnemyIdleState());
                        break;
                    
                }
            }
            
        }
        else
        {
            enemy.ChangeState(new EnemyWalkState());
        }
       
    }

    public void Exit(Enemy enemy)
    {
      
    }
}