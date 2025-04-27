using UnityEngine;


public class BossMoveState : IState<Boss>
{
    private Vector3 _targetPosition;
    private bool _finished;

    public void Enter(Boss boss)
    {
       
        if (boss.SkipNextMove)
        {
            boss.ChangeState(new BossIdleState());
            return;
        }

      
        if (boss.CheckDistance())
        {
            boss.ChangeState(new BossIdleState());
            return;
        }

        
        boss.Animator.SetBool("1_Move", true);
        _finished = false;
        _targetPosition = boss.transform.position 
                          + boss.Dircetion * boss.tileDistance;
    }

    public void Update(Boss boss)
    {
        if (_finished) return;

       
        float step = boss.BossMoveSpeed * Time.deltaTime;
        boss.transform.position = Vector3.MoveTowards(
            boss.transform.position,
            _targetPosition,
            step
        );

      
        if (Vector3.Distance(boss.transform.position, _targetPosition) < 0.01f)
        {
            _finished = true;
            boss.UpMoveCount();
            boss.Animator.SetBool("1_Move", false);

            
            boss.ChangeState(new BossIdleState());
        }
    }

    public void Exit(Boss boss)
    {
      
        boss.Animator.SetBool("1_Move", false);
    }
}