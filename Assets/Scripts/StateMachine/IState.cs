using UnityEngine;

public interface IState
{
    void OnEntry();
    void StateUpdate();
    void OnExit();
}
