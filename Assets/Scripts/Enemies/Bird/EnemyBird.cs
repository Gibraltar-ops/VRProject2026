using UnityEngine;

public class EnemyBird : EnemyParentClass
{

    [Header("Circleing")]
    public float _circleRadius;
    public float _circleSpeed;
    public float _heightAboveTarget;

    [Header("Attacking")]

    public float _attackingSpeed = 50f;


    public float currentRadius;

    public StateMachine _stateMachine = new StateMachine();
    public BirdAttackState _birdAttackState;
    public BirdCircleState _birdCircleState;


    override public void Awake()
    {
        SetInitTarget();
        _birdAttackState     = new BirdAttackState(this, target, currentRadius);
        _birdCircleState     = new BirdCircleState(this);
        //_birdDisengageState  = new BirdDisengageState();

        _stateMachine.EnterState(_birdCircleState);
        
    }

    override public void Update()
    {
        _stateMachine._currentState.StateUpdate();
    }

    void OnEnable()
    {
        BalloonTree.OnBalloonPopped += HandleTargetChange;
    }

    void OnDisable()
    {
        BalloonTree.OnBalloonPopped -= HandleTargetChange;
    }

    private void HandleTargetChange(BalloonInfo balloon)
    {
        _stateMachine.EnterState(_birdCircleState);
    }
}
