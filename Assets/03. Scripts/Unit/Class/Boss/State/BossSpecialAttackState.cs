// BossDropAttackState.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropAttackState : IState<Boss>
{
    public void Enter(Boss boss)
    {
        // 1) Jump 애니메이션
        Debug.Log(boss.MoveCount +"현재 움직인 횟수");
        boss.Animator.SetTrigger("Jump");
        // 2) 이 상태 동안 스킬·공격 불가
        boss.InitializeAttack();
        // 3) 코루틴으로 1초 뒤 랜드 및 후처리
        boss.StartCoroutine(JumpAndLand(boss));
    }

    private IEnumerator JumpAndLand(Boss boss)
    {
        // 점프 중 플래그
        boss.Jumping = true;
        
        
       
        
        yield return new WaitForSeconds(1f);
        if (boss.MoveCount >= 5)
        {
            boss.transform.position = boss.InitialPosition;
            boss.MoveCount = 0;
        }

      
       

        // Land 애니
        boss.Animator.SetTrigger("Land");
        yield return new WaitForSeconds(1f);
        
        
        if (boss.SkipNextMove)
        {
            boss.Jumping = false;
            boss.ChangeState(new BossStunState()); 
            yield break;
        }

      
        SoundManager.Instance.PlaySfx(boss.BossData.SkillSound[0],boss.transform.position);
        // 기존 랜딩 후 아군 처리
        List<Ally> front = AllyPoolManager.Instance.GettLineObject_Spawned(LineType.Front);
        if (front.Count < 2)
        {
            boss.DestroyAllAllies();
            boss.DealDamageToPlayer(30);
           
        }
        else
        {
            boss.DespawnRandomFrontAllies(front, 2);
            SoundManager.Instance.PlaySfx(boss.BossData.SkillSound[0],boss.transform.position);
        }
        
        
        boss.Jumping = false;
        // 다음은 Idle 상태로 복귀
        boss.ChangeState(new BossIdleState());
    }

    public void Update(Boss boss)
    {
        // 모든 로직이 코루틴 안에서 처리되므로 아무것도 안 해도 됩니다.
    }

    public void Exit(Boss boss)
    {
        // 혹시 남아 있는 플래그 클리어
        boss.Jumping = false;
        boss.SkipNextMove = false;
    }
}