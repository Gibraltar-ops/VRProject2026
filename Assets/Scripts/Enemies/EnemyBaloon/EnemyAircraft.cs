using UnityEngine;

public class EnemyAircraft : EnemyParentClass
{
    [Header("References")]
    public BalloonTree _playerBalloonTree;
    public EnemySpawner _enemySpawner;

    [Header("StateMachine")]
    public StateMachine _statemachine = new StateMachine();
    public EnemyFireAtBird          _enemyFireAtBird;
    public EnemyFireAtPlayerState   _enemyFireAtPlayerState;
    public EnemyRefillState         _enemyRefillState;

    [Header("Stats")]
    private int enemiesKilled;
    public float accuracy {get;}          = .6f;
    public float aimTime {get;}           = 3f;
    public float timeToFillBalloon {get;} = 5f;

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
        _playerBalloonTree = GameManager.instance.PlayerBaloonTree;
        _statemachine.EnterState(_enemyRefillState);
    }

    public override void Update()
    {
        _statemachine._currentState.StateUpdate();
        UpdateAltitude();
    }

    private void UpdateAltitude()
    {
        //Handle enemy altitude change

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

        switch (_playerBalloonTree.ActiveBalloons)
        {
            case < 10:
                _playerChance = 0;
                _refillChance = _remaningChance;
                break;
            
            case < 15:
                _playerChance = 1/3 * _remaningChance;
                _refillChance = 2/3 * _remaningChance;
                break;

            case 15:
                _playerChance = .5f * _remaningChance;
                _refillChance = .5f * _remaningChance;
                break;

            case < 18:
                _playerChance = 2/3 * _remaningChance;
                _refillChance = 1/3 * _remaningChance;
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
        _statemachine.ChangeState(MakeDecition());
    }

}
