using UnityEngine;

public class StateMachine<T> where T : MonoBehaviour
{
    private IState<T> _currentState;
    private T _owner;

    public StateMachine(T owner)
    {
        _owner = owner;
    }

    public void ChangeState(IState<T> newState)
    {
        _currentState?.Exit(_owner);
        _currentState = newState;
        _currentState.Enter(_owner);
    }

    public void Update()
    {
        _currentState?.Update(_owner);
    }
}