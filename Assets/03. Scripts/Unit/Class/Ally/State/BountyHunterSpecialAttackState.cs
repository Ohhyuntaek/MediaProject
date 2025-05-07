using UnityEngine;

public class BountyHunterSpecialAttackState : IState<Ally>
{
    private Transform _hitPos;
    private bool _finished = false;

    public BountyHunterSpecialAttackState(Transform hitpos)
    {
        _hitPos = hitpos;
    }

    public void Enter(Ally ally)
    {
        _finished = false;
        ally.Animator.SetTrigger("5_SpecialAttack");
    }

    public void Update(Ally ally)
    {
        // 매프레임 애니메이션 상태 체크
        TranstionTo(ally);
    }

    private void TranstionTo(Ally ally)
    {
        var state = ally.Animator.GetCurrentAnimatorStateInfo(0);
        // 스테이트 이름 확인, 애니메이션 끝난 시점
        if (state.IsName("SpecialAttack") && state.normalizedTime >= 0.9f && !_finished)
        {
            _finished = true;
            SpawnBomb(ally);
            // 공격 후 Idle로 복귀
            ally.ChangeState(new AllyIdleState(2f));
        }
    }

    public void Exit(Ally ally)
    {
        // 필요한 정리 작업이 있으면
    }

    private void SpawnBomb(Ally ally)
    {
        // 1) 프리팹 가져오기
        GameObject bombPrefab = ally.UnitData.SkillEffect[0];
        if (bombPrefab == null)
        {
            Debug.LogError("Bomb Prefab이 설정되지 않았습니다!");
            return;
        }

        // 2) 씬에 인스턴스 생성 (위치, 회전 지정 가능)
        GameObject bombInstance = Object.Instantiate(
            bombPrefab,
            _hitPos.position,
            Quaternion.identity
        );

        // 3) BountyHunterBomb 컴포넌트 가져오거나 추가
        var bombComp = bombInstance.GetComponent<BountyHunterBomb>();
        if (bombComp == null)
            bombComp = bombInstance.AddComponent<BountyHunterBomb>();

        // 4) 초기화 호출
        bombComp.Init(ally.UnitData.DetectionPatternSo, _hitPos);
    }
}