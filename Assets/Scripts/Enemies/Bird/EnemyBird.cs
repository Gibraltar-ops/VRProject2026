using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyBird : EnemyParentClass
{
    public enum BirdStateEnum
    {
        Flying,
        Attacking,
        Dead
    }

    public BirdStateEnum _birdStateEnum;

    public Health _health;
    public GameObject particleVFX;
    public bool isAlive;

    [Header("Circleing")]
    public float _circleRadius;
    public float _circleSpeed;
    public float _heightAboveTarget;

    [Header("Attacking")]
    public float _attackingSpeed = 5f;

    public float currentRadius;
    public int circleDirection;

    public StateMachine _stateMachine = new StateMachine();
    public BirdAttackState _birdAttackState;
    public BirdCircleState _birdCircleState;
    public BirdDeadState _birdDeadState;

    public bool hasAttacked;

    public BirdStateEnum GetBirdStateEnum()
    {
        return _birdStateEnum;
    }

    override public void Start()
    {
        _birdStateEnum = BirdStateEnum.Flying;
    }

    void OnEnable()
    {
        _birdAttackState = new BirdAttackState(this, target);
        _birdCircleState = new BirdCircleState(this);
        _birdDeadState = new BirdDeadState(this);

        isAlive = true;
        _stateMachine.EnterState(_birdCircleState);
        _birdStateEnum = BirdStateEnum.Flying;

        BalloonTree.OnBalloonPopped += HandleTargetChange;
        _health.Die += EnterDeadState;
    }

    void OnDisable()
    {
        BalloonTree.OnBalloonPopped -= HandleTargetChange;
        _health.Die -= EnterDeadState;
    }

    public string GetCurrentStateDebug()
    {
        if(_stateMachine._currentState == _birdAttackState)
        {
            return "Attack - State";
        }
        if (_stateMachine._currentState == _birdCircleState)
        {
            return "Circle - State";
        }
        if (_stateMachine._currentState == _birdDeadState)
        {
            return "Dead - State";
        }
        return "No State";
    }

    public string GetCurrentAlive()
    {
        if (isAlive) { return "Alive"; }
        else { return "Dead"; }
    }

    override public void Update()
    {
        if (isAlive == false) 
        {
            if (_birdDeadState != _stateMachine._currentState)
            {
                ChangeBirdState(_birdDeadState);
                Debug.LogWarning("Extra Change state run - to dead state");
            }
        }
        _stateMachine._currentState.StateUpdate();
    }

    private void HandleTargetChange(BalloonInfo balloon)
    {
        ChangeBirdState(_birdCircleState);
    }

    private void EnterDeadState()
    {
        Instantiate(particleVFX, transform.position, Quaternion.identity);
        ChangeBirdState(_birdDeadState);
        isAlive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasAttacked)
        {
            if (other.gameObject.CompareTag("Balloon"))
            {
                _targetBalloonTree.PopBalloon(target);
                hasAttacked = true;
            }
            
        }
    }

    private void OnCollisionEnter(Collision other)
    {
            if (other.gameObject.CompareTag("Balloon"))
            {
                _targetBalloonTree.PopBalloon(target);
                hasAttacked = true;
            }
    }

    public void ChangeBirdState(IState newState)
    {
        if (isAlive)
        {
            _stateMachine.ChangeState(newState);
        }
        else
        {
            _birdStateEnum = BirdStateEnum.Dead;
            Debug.LogWarning("Bird is dead and trying to change State!");
        }
    }
}
