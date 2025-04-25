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
        yield return new WaitForSeconds(1f); // 1초 대기

        // 랜딩 애니메이션
        boss.Animator.SetTrigger("Land");

        // CC 당한 경우 다음 이동 스킵
        if (boss.SkipNextMove)
        {
            
        }

        // 랜딩 시 전열 아군 체크
        List<Ally> front = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Front);
        if (front.Count < 2)
        {
            boss.DestroyAllAllies();
            boss.DealDamageToPlayer(30); // 30 데미지
        }
        else
        {
            boss.DespawnRandomFrontAllies(front, 2);
        }

        // 끝나면 버프 스테이트로
        boss.ChangeState(new BossIdleState());
    }

    public void Update(Boss boss)
    {
        // 아무것도 안 함. Enter에서 코루틴이 로직을 모두 수행
    }

    public void Exit(Boss boss)
    {
        
    }
}