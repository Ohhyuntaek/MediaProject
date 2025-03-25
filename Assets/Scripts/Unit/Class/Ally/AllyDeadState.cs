using UnityEngine;

public class AllyDeadState : IState
{
    private Ally _ally;

    public AllyDeadState(Ally ally)
    {
        _ally = ally;
    }

    public void Enter()
    {
        _ally.Animator?.SetTrigger("4_Death");
        _ally.PerformDie();
    }

    public void Update() { }
    public void Exit() { }
}

