using System.Collections.Generic;
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

    [Header("Sway and rotation")]
    private float targetRotation;
    private float rotationSpeed;
    private float swayTimer;

    private List<Vector3> swayPoints = new List<Vector3>();
    private Vector3 currentSwayPoint;
    private float swaySpeed;
    [SerializeField] private float swayRadius = 5;
    private float swayChangeTime;
    private float swayChange;

    private float timeSinceLastBoost;
    private float boostCooldown = 10;
    [SerializeField]private float aiCapDiff;


    public override void Awake()
    {
        _enemyFireAtBird         = new EnemyFireAtBird(this, _enemySpawner);
        _enemyFireAtPlayerState  = new EnemyFireAtPlayerState(this);
        _enemyRefillState        = new EnemyRefillState(this);
    }


    public override void Start()
    {
        _targetBalloonTree = GameManager.instance.PlayerBaloonTree;
        _statemachine.EnterState(_enemyRefillState);
        SetSwayPoints();

        enemyAltitude = 0;
        enemyVelocity = 0;

        startPos = gameObject.transform.position;
        targetRotation = gameObject.transform.eulerAngles.y;

        timeSinceLastBoost = 0;
        swayTimer = 0;

        relativeDistance = 0;

        aiCapDiff = 0;
    }

    public override void Update()
    {
        _statemachine._currentState.StateUpdate();
        timeSinceLastBoost += Time.deltaTime;
        UpdateAltitude();
        UpdateRotation();
        SetRangePhase();
        
        aiCapDiff = Mathf.Lerp(1.25f, .75f, Mathf.InverseLerp(-RelativeDistanceCap, RelativeDistanceCap, relativeDistance));
    }

    private void UpdateAltitude()
    {
        enemyVelocity = GameManager.instance.CalculateVelocity(ownBalloonTree.ActiveBalloons, enemyVelocity, false, aiCapDiff);
        enemyAltitude += enemyVelocity * Time.deltaTime;
        swayChangeTime += Time.deltaTime;
        relativeDistance = enemyAltitude - GameManager.instance.Altitude;
        

        if(enemyAltitude <= 0)
        {
            enemyAltitude = 0;
        }

        //Top cap
        if(relativeDistance >= RelativeDistanceCap)
        {
            relativeDistance = RelativeDistanceCap;
            enemyAltitude = GameManager.instance.Altitude + RelativeDistanceCap;
        }

        //Bottom cap
        if(relativeDistance <= -1 * RelativeDistanceCap)
        {
            relativeDistance = -1 * RelativeDistanceCap;
            enemyAltitude = GameManager.instance.Altitude - RelativeDistanceCap;
        }

        
        //Dont fall trough ground check
        if(relativeDistance <= GameManager.instance.GroundAltitude)
        {
            relativeDistance = GameManager.instance.GroundAltitude;
        }
        
        swaySpeed = Mathf.Lerp(.10f, .4f, Mathf.Sin(swayTimer) + 1f) / 2f;
        if(swayChangeTime >= swayChange)
        {
            GetNewSwayPoint();
            swayChangeTime = 0;
            swayChange = Random.Range(10, 25);
        }

        gameObject.transform.position = new Vector3
        (
            Mathf.Lerp(gameObject.transform.position.x, currentSwayPoint.x, swaySpeed*Time.deltaTime),
            relativeDistance,
            Mathf.Lerp(gameObject.transform.position.z, currentSwayPoint.z, swaySpeed*Time.deltaTime)
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

    #region Decitions

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
        accuracy = .6f;
        aimTime = 3f;
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
            aimTime = .5f;
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
    #endregion

    private void UpdateRotation()
    {
        if(enemyAltitude < 5)
        {
            return;
        }
        if(Quaternion.Angle(transform.rotation, Quaternion.Euler(0, targetRotation, 0)) < 1f)
        {
            targetRotation = Random.Range(0, 360);
        }

        swayTimer += Time.deltaTime;
        rotationSpeed = Mathf.Lerp(2f, 8f, Mathf.Sin(swayTimer) + 1f) / 2f;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, targetRotation, 0),
            rotationSpeed * Time.deltaTime
        );
    }

    private void SetSwayPoints()
    {
        for (int i = 0; i <= 360; i += 30)
        {
            swayPoints.Add(gameObject.transform.position + new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i))*swayRadius);
        }
    }

    private void GetNewSwayPoint()
    {
        int index = Random.Range(0, swayPoints.Count);
        currentSwayPoint = swayPoints[index];
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