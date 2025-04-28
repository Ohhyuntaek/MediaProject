using UnityEngine;

public class BossIdleState : IState<Boss>
{
    private float _timer;
    private bool _finished;
    
    public void Enter(Boss boss)
    {   
        Debug.Log("아이들 상태 진입 현재 Move" + boss.MoveCount);
        boss.Animator.SetBool("1_Move", false);
        _timer = boss.MoveInterval;
        _finished = false;
    }

    public void Update(Boss boss)
    {
        _timer -= Time.deltaTime;
        if (_finished) return;

        if (_timer <= 0f)
        {
           
            if (boss.MoveCount!=0 && boss.MoveCount%2==0 && !boss.LastSpecialAttack )
            {   
                
                _finished = true;
                boss.LastSpecialAttack = true;
                boss.ChangeState(new BossDropAttackState());
                return;
            }

            if (boss.CheckDistance())
            {
                _finished = true;
                boss.ChangeState(new BossDropAttackState());
                return;
            }
            

            
            if (boss.CanAttack)
            {
                _finished = true;
                int rand = Random.Range(0, 2);
                if (rand == 0)
                    boss.ChangeState(new BossAttack1State());
                else
                    boss.ChangeState(new BossAttack2State());
            }
            else
            {
                _finished = true;
                boss.ChangeState(new BossMoveState());
            }
        }
    }

    public void Exit(Boss boss) { }
}