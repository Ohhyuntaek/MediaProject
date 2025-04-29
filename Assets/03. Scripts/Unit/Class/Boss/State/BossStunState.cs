using System.Collections;
using UnityEngine;

public class BossStunState : IState<Boss>
{
    private readonly float _duration = 5f;
    private Coroutine _stunCoroutine;

    public void Enter(Boss boss)
    {
        // 1) 이동 애니메이션 끄기
        boss.Animator.SetBool("1_Move", false);

        // 2) 이펙트만 활성화
        if (boss.StunEffect != null)
            boss.StunEffect.SetActive(true);

        // 3) 5초 후 해제
        _stunCoroutine = boss.StartCoroutine(StunTimer(boss));
    }

    private IEnumerator StunTimer(Boss boss)
    {
        yield return new WaitForSeconds(_duration);

        // 효과 끄기
        if (boss.StunEffect != null)
            boss.StunEffect.SetActive(false);

        // 공격 재개 준비
        boss.InitializeAttack();

        // 상태 전환
        boss.ChangeState(new BossIdleState());
    }

    public void Update(Boss boss)
    {
        // 스턴 중에는 아무 동작 없음
    }

    public void Exit(Boss boss)
    {
        // 남은 코루틴이 있다면 정리
        if (_stunCoroutine != null)
        {
            boss.StopCoroutine(_stunCoroutine);
            _stunCoroutine = null;
        }
        // 안전하게 이펙트 끄기
        if (boss.StunEffect != null)
            boss.StunEffect.SetActive(false);
    }
}