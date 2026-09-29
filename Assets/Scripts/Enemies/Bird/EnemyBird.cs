using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyBird : EnemyParentClass
{
    public Health _health;

    [Header("Circleing")]
    public float _circleRadius;
    public float _circleSpeed;
    public float _heightAboveTarget;

    [Header("Attacking")]
    public float _attackingSpeed = 50f;

    public float currentRadius;
    public int circleDirection;

    public StateMachine _stateMachine = new StateMachine();
    public BirdAttackState _birdAttackState;
    public BirdCircleState _birdCircleState;
    public BirdDeadState _birdDeadState;

    override public void Start()
    {
        _birdAttackState     = new BirdAttackState(this, target);
        _birdCircleState     = new BirdCircleState(this);
        _birdDeadState       = new BirdDeadState(this);

        _stateMachine.EnterState(_birdCircleState);
        
    }

    override public void Update()
    {
        _stateMachine._currentState.StateUpdate();
    }

    void OnEnable()
    {
        BalloonTree.OnBalloonPopped += HandleTargetChange;
        _health.Die += EnterDeadState;
    }

    void OnDisable()
    {
        BalloonTree.OnBalloonPopped -= HandleTargetChange;
        _health.Die -= EnterDeadState;
    }

    private void HandleTargetChange(BalloonInfo balloon)
    {
        _stateMachine.EnterState(_birdCircleState);
    }

    private void EnterDeadState()
    {
        _stateMachine.ChangeState(_birdDeadState);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Balloon"))
        {
            Debug.Log("Pop");
        }
    }

}
