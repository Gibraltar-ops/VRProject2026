using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public GameObject target;

    private float speed = 5f;


    private StateMachine _stateMachine = new StateMachine();
    private BirdAttackState _birdAttackState;
    private BirdCircleState _birdCircleState;
    private BirdDisengageState _birdDisengageState;


    void Awake()
    {
        _birdAttackState     = new BirdAttackState();
        _birdCircleState     = new BirdCircleState(target, this);
        _birdDisengageState  = new BirdDisengageState();

        _stateMachine.EnterState(_birdCircleState);
    }

    void Update()
    {
        _stateMachine._currentState.StateUpdate();
    }
}
