
public class KillrenSpecialAttackState : IState<Ally>
{   
    private bool _finished = false;
    public void Enter(Ally ally)
    {
        ally.Animator.SetTrigger("5_SpecialAttack");
    }

    public void Update(Ally ally)
    {
        TranstionTo(ally);
    }
    private void TranstionTo(Ally ally)
    {
        var state = ally.Animator.GetCurrentAnimatorStateInfo(0);
        // 스테이트 이름 확인, 애니메이션 끝난 시점
        if (state.IsName("Killren") && state.normalizedTime >= 0.9f && !_finished)
        {
            _finished = true;
            ally.PerformSkill();
            ally.ChangeState(new AllyIdleState(2f));
        }
    }

    public void Exit(Ally ally)
    {
        throw new System.NotImplementedException();
    }
}
