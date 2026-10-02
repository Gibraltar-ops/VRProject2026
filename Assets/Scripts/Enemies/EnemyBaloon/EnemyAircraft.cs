using UnityEngine;

public class EnemyAircraft : EnemyParentClass
{
    [Header("References")]
    public BalloonTree ownBalloonTree;
    public EnemySpawner _enemySpawner;

    [Header("StateMachine")]
    public StateMachine _statemachine = new StateMachine();
    public EnemyFireAtBird          _enemyFireAtBird;
    public EnemyFireAtPlayerState   _enemyFireAtPlayerState;
    public EnemyRefillState         _enemyRefillState;

    [Header("Stats")]
    public float accuracy {get;}          = .6f;
    public float aimTime {get;}           = 3f;
    public float timeToFillBalloon {get;} = 5f;

    [Header("Altitude")]
    private float enemyAltitude;
    private float enemyVelocity;
    private Vector3 startPos;

    [Header("Decitions")]
    private float _birdChance;
    private float _refillChance;
    private float _playerChance;
    private float _remaningChance;


    public override void Awake()
    {
        _enemyFireAtBird         = new EnemyFireAtBird(this, spawnerForThis);
        _enemyFireAtPlayerState  = new EnemyFireAtPlayerState(this);
        _enemyRefillState        = new EnemyRefillState(this);
    }


    public override void Start()
    {
        _targetBalloonTree = GameManager.instance.PlayerBaloonTree;
        _statemachine.EnterState(_enemyRefillState);
        enemyAltitude = 0;
        enemyVelocity = 0;
        startPos = gameObject.transform.position;
    }

    public override void Update()
    {
        _statemachine._currentState.StateUpdate();
        UpdateAltitude();
    }

    private void UpdateAltitude()
    {
        enemyVelocity = GameManager.instance.CalculateVelocity(ownBalloonTree.ActiveBalloons, enemyVelocity);
        enemyAltitude += enemyVelocity * Time.fixedDeltaTime;

        if(enemyAltitude <= 0)
        {
            enemyAltitude = 0;
            return;
        }
        gameObject.transform.position = new Vector3
        (
            startPos.x,
            enemyAltitude - GameManager.instance.Altitude,
            startPos.z
        );
        

    }
    private IState MakeDecition()
    {
        int currentBirds = _enemySpawner.enemiesSpawned.Count;
        switch (currentBirds)
        {
            case <= 0:
                _birdChance = 0;
                break;

            case 1:
                _birdChance = 20;
                break;

            case 2:
                _birdChance = 40;
                break;
                
            case 3:
                _birdChance = 60;
                break;

            case 4:
                _birdChance = 80;   
                break;
                
            case >= 5:
                _birdChance = 100;
                break;
        }
        _remaningChance = 100 -_birdChance;

        if(_remaningChance <= 0)
        {
            return _enemyFireAtBird;
        }

        switch (ownBalloonTree.ActiveBalloons)
        {
            case < 10:
                _playerChance = 0;
                _refillChance = _remaningChance;
                break;
            
            case < 15:
                _playerChance = 1f / 3f * _remaningChance;
                _refillChance = 2f / 3f * _remaningChance;
                break;

            case 15:
                _playerChance = .5f * _remaningChance;
                _refillChance = .5f * _remaningChance;
                break;

            case < 18:
                _playerChance = 2f / 3f * _remaningChance;
                _refillChance = 1f / 3f * _remaningChance;
                break;

            case >= 18:
                _playerChance = _remaningChance;
                _refillChance = 0;
                break;
        }

        float decition = Random.Range(1,101);
        if(decition < _birdChance)
        {
            return _enemyFireAtBird;
        }
        else if(decition < _birdChance + _refillChance)
        {
            return _enemyRefillState;
        }
        else
        {
            return _enemyFireAtPlayerState;
        }


    }

    public void ChangeTask()
    {
        IState newState = MakeDecition();
        _statemachine.ChangeState(newState);
        Debug.Log("Changed task to: " + newState);
    }

}
