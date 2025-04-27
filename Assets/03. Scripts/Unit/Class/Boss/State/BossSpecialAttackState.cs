// BossDropAttackState.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropAttackState : IState<Boss>
{
    private bool _started = false;

    public void Enter(Boss boss)
    {
        // 점프 애니메이션
        boss.Animator.SetTrigger("Jump");
        // 점프하는 동안 다른 공격을 하지 못하게
        boss.InitializeAttack();   
        // 코루틴으로 1초 뒤 랜딩 처리
        boss.StartCoroutine(JumpAndLand(boss));
    }

    private IEnumerator JumpAndLand(Boss boss)
    {
        _started = true;
        boss.Jumping = true;
        yield return new WaitForSeconds(1f);
        if (boss.SkipNextMove)
        {
            boss.ChangeState(new BossStunState());
            boss.Jumping = false;
            Debug.Log("너 스킵");
        } 
        boss.Jumping = false;

        // 랜딩 애니메이션
        boss.Animator.SetTrigger("Land");
        

        // 랜딩 시 전열 아군 체크
        List<Ally> front = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Front);
        if (front.Count < 2 &&!boss.SkipNextMove)
        {
            boss.DestroyAllAllies();
            boss.DealDamageToPlayer(30); // 30 데미지
        }
        else if(front.Count >= 2 && !boss.SkipNextMove)
        {
            boss.DespawnRandomFrontAllies(front, 2);
        }

        
        boss.ChangeState(new BossIdleState());
    }

    public void Update(Boss boss)
    {
        // 아무것도 안 함. Enter에서 코루틴이 로직을 모두 수행
    }

    public void Exit(Boss boss)
    {
        boss.Jumping = false;
        boss.SkipNextMove = false;
    }
}