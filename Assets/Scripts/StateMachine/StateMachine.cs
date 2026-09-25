using UnityEngine;

public class StateMachine
{
    public IState _currentState;

    public void ChangeState(IState newState)
    {
        _currentState.OnExit();
        _currentState = newState;
        _currentState.OnEntry();
    }

    public void EnterState(IState state)
    {
        _currentState = state;
        _currentState.OnEntry();
    }
}
