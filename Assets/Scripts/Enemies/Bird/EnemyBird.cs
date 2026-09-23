using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public GameObject target;
    public float currentRadius;

    private StateMachine _stateMachine = new StateMachine();
    private BirdAttackState _birdAttackState;
    private BirdCircleState _birdCircleState;
    private BirdDisengageState _birdDisengageState;

    private float timer;



    void Awake()
    {
        _birdAttackState     = new BirdAttackState(this, target, currentRadius);
        _birdCircleState     = new BirdCircleState(this);
        _birdDisengageState  = new BirdDisengageState();

        _stateMachine.EnterState(_birdCircleState);

        timer = 0;

        
    }

    void Update()
    {
        _stateMachine._currentState.StateUpdate();
        timer += Time.deltaTime;
        if(timer > 5f && _stateMachine._currentState != _birdAttackState)
        {
            _stateMachine.ChangeState(_birdAttackState);
        }
    }
}
