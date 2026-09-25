using UnityEngine;

public class EnemyBird : MonoBehaviour
{

    [Header("Circleing")]
    public float _circleRadius;
    public float _circleSpeed;
    public float _heightAboveTarget;

    [Header("Attacking")]

    public float _attackingSpeed = 50f;


    public GameObject target;
    public float currentRadius;

    public StateMachine _stateMachine = new StateMachine();
    public BirdAttackState _birdAttackState;
    public BirdCircleState _birdCircleState;

    private float timer;



    void Awake()
    {
        _birdAttackState     = new BirdAttackState(this, target, currentRadius);
        _birdCircleState     = new BirdCircleState(this);
        //_birdDisengageState  = new BirdDisengageState();

        _stateMachine.EnterState(_birdCircleState);
        
    }

    void Update()
    {
        _stateMachine._currentState.StateUpdate();
    }
}
