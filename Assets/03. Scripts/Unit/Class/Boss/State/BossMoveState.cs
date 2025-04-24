using UnityEngine;
using System.Collections;

public class BossMoveState : IState<Boss>
{
    private Vector3 _targetPosition;
    private bool _finished = false;
    public void Enter(Boss boss)
    {
        if (boss.SkipNextMove)
        {
            boss.ChangeState(new BossIdleState());
            return;
        }
        boss.Animator.SetBool("1_Move", true);
        Vector3 dir = boss.Dircetion;
        _targetPosition = boss.transform.position + dir * boss.tileDistance;
    }

    public void Update(Boss boss)
    {
        float step = boss.BossMoveSpeed * Time.deltaTime;
        boss.transform.position = Vector3.MoveTowards(
            boss.transform.position,
            _targetPosition,
            step
        );

        TransionTo(boss);
    }

    public void TransionTo(Boss boss)
    {
        if (Vector3.Distance(boss.transform.position, _targetPosition) < 0.01f &&!_finished)
        {
            // 이동 카운트 증가
            _finished = true;
            boss.UpMoveCount();

            // 6회 이상 이동했으면 플레이어 접근, 아니면 다시 Idle
            if (boss.MoveCount >= 6)
                boss.ChangeState(new BossIdleState());
            else
                boss.ChangeState(new BossIdleState());
        }
    }
    

    public void Exit(Boss boss)
    {
        
    }
}