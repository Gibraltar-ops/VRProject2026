using System.Data.Common;
using UnityEngine;

public class EnemyAircraft : EnemyParentClass
{
    private enum RangePhase
    {
        FarAbove,
        Above,
        Sweetspot,
        Below,
        FarBelow
    }
    [Header("References")]
    public BalloonTree ownBalloonTree;
    public EnemySpawner _enemySpawner;

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private float RelativeDistanceCap = 50;
    [SerializeField] private RangePhase rangePhase;

    [Header("StateMachine")]
    public StateMachine _statemachine = new StateMachine();
    public EnemyFireAtBird          _enemyFireAtBird;
    public EnemyFireAtPlayerState   _enemyFireAtPlayerState;
    public EnemyRefillState         _enemyRefillState;

    [Header("Stats")]
    public float accuracy {get; private set;}    = .6f;
    public float aimTime {get; private set;}     = 3f;
    public float timeToFillBalloon {get;}        = 5f;

    [Header("Altitude")]
    private float enemyAltitude;
    private float enemyVelocity;
    private Vector3 startPos;
    [SerializeField] private float relativeDistance;

    private float timeSinceLastBoost;
    private float boostCooldown = 10;


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
        timeSinceLastBoost = 0;
        relativeDistance = 0;
    }

    public override void Update()
    {
        _statemachine._currentState.StateUpdate();
        timeSinceLastBoost += Time.deltaTime;
        UpdateAltitude();
        SetRangePhase();

    }

    private void UpdateAltitude()
    {
        enemyVelocity = GameManager.instance.CalculateVelocity(ownBalloonTree.ActiveBalloons, enemyVelocity, false);
        enemyAltitude += enemyVelocity * Time.deltaTime;
        relativeDistance = enemyAltitude - GameManager.instance.Altitude;

        if(enemyAltitude <= 0)
        {
            enemyAltitude = 0;
        }

        //Top cap
        if(relativeDistance >= RelativeDistanceCap)
        {
            relativeDistance = RelativeDistanceCap;
        }

        //Bottom cap
        if(relativeDistance <= -1 * RelativeDistanceCap)
        {
            relativeDistance = -1 * RelativeDistanceCap;
        }

        
        //Dont fall trough ground check
        if(relativeDistance <= -1 * GameManager.instance.Altitude)
        {
            relativeDistance = -GameManager.instance.Altitude;
        }
        

        gameObject.transform.position = new Vector3
        (
            startPos.x,
            relativeDistance,
            startPos.z
        );

        if(relativeDistance >= RelativeDistanceCap && timeSinceLastBoost >= boostCooldown)
        {
            if (debug)
            {
                Debug.Log("Activated penalty for AI");
            }
           _enemySpawner.SpawnDisaster(Mathf.RoundToInt(relativeDistance/10));
           timeSinceLastBoost = 0;
        }
        if(relativeDistance <= -RelativeDistanceCap && timeSinceLastBoost >= boostCooldown)
        {
            if (debug)
            {
                Debug.Log("Activated boost for AI");
            }
            ownBalloonTree.AddBalloon();
            ownBalloonTree.AddBalloon();
            ownBalloonTree.AddBalloon();
            timeSinceLastBoost = 0;
            
        }
    }
    private IState MakeDecition()
    {
        switch (rangePhase)
        {
            case RangePhase.FarAbove:
                return FarAboveDec();
            
            case RangePhase.Above:
                return AboveDec();

            case RangePhase.Sweetspot:
                return SweetSpotDec();

            case RangePhase.Below:
                _enemySpawner.SetBoostValue(false);
                return BelowDec();
            
            case RangePhase.FarBelow:
                _enemySpawner.SetBoostValue(true);
                return FarBelowDec();

            default:
                return _enemyRefillState;

        }
    }

    private IState FarAboveDec()
    {
        return _enemyFireAtBird;
    }

    private IState AboveDec()
    {
        if(ownBalloonTree.ActiveBalloons < 12)
        {
            return _enemyRefillState;
        }
        else
        {
            return _enemyFireAtPlayerState;
        }
    }

    private IState SweetSpotDec()
    {
        bool change = minBalloonCheck();
        if (!change)
        {
            return _enemyRefillState;
        }
    

        float chance = Random.Range(0f,1f);
        if(chance < 1f / 3f)
        {
            return _enemyRefillState;
        }
        else if(chance < 2f / 3f)
        {
            return _enemyFireAtBird;
        }
        else
        {
            return _enemyFireAtPlayerState;
        }

    }

    private IState BelowDec()
    {
        if(_enemySpawner.enemiesSpawned.Count <= 0)
        {
            return _enemyRefillState;
        }

        float probabilityForBird = Mathf.InverseLerp(-50f, -20f, relativeDistance);

        if(Random.value < probabilityForBird)
        {
            return _enemyFireAtBird;
        }
        else
        {
            return _enemyRefillState;
        }
    }

    private IState FarBelowDec()
    {
        if(_enemySpawner.enemiesSpawned.Count > 0)
        {
            accuracy = 1;
            aimTime = 2;
            return _enemyFireAtBird;
        }
        else
        {
            accuracy = .6f;
            aimTime = 3;
            return _enemyRefillState;
        }
    }

    private bool minBalloonCheck()
    {
        if(ownBalloonTree.ActiveBalloons <= 10)
        {
            if (debug)
            {
                Debug.Log("Balloon Count < 10, proritizing Balloons");
            }
            return false;
        }
        return true;
    }

    private void SetRangePhase()
    {
        switch (relativeDistance)
        {
            case <= -50:
                rangePhase = RangePhase.FarBelow;
                break;
            
            case <= -20:
                rangePhase = RangePhase.Below;
                break;
            
            case <= 20:
                rangePhase = RangePhase.Sweetspot;
                break;
            
            case <= 30:
                rangePhase = RangePhase.Above;
                break;
            
            case > 30:
                rangePhase = RangePhase.FarAbove;
                break;
        }
        
    }

    public void ChangeTask()
    {
        IState newState = MakeDecition();
        _statemachine.ChangeState(newState);
        if (debug)
        {
            Debug.Log("Changed task to: " + newState);
        }
    }

}