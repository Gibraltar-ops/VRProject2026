using UnityEngine;

public class BirdCircleState : IState
{
    public BirdCircleState(EnemyBird bird)
    {
        this.bird   = bird;
    }
        
    
    [Header("Circling")]
    private float _circleMaxRadius = 15;
    private float _cirecleMinRadius = 5;

    private float _circleMaxSpeed = 2f;
    private float _circleMinSpeed = .5f;

    private float _minHeightAboveTarget = 0;
    private float _maxHeightAboveTarget = 4;

    private int _clockwise;

    private float angle;
    private EnemyBird bird;

    float timer;

    public void OnEntry()
    {
        bird._circleRadius       = Random.Range(_cirecleMinRadius, _circleMaxRadius);
        bird._circleSpeed        = Random.Range(_circleMinSpeed, _circleMaxSpeed);
        bird._heightAboveTarget  = Random.Range(_minHeightAboveTarget, _maxHeightAboveTarget);
        _clockwise               = Random.Range(1,3);
        if(_clockwise == 2)
        {
            _clockwise = -1;
        }

        timer = 10f;
    }

    public void OnExit()
    { 
    }

    public void StateUpdate()
    {
        if(bird.target == null)
        {
            bird.target = bird.targetBalloonTree.GetClosestAvailableBalloon(bird.gameObject.transform.position);
            if(bird.target == null)
            {
                bird.target = bird.targetBalloonTree.GetOutmostBalloon();
            }
        }
        CircleTarget();
        UpdateRotation();
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            bird._stateMachine.ChangeState(bird._birdAttackState);
        }
    }

    private void CircleTarget()
    {
    angle += bird._circleSpeed * Time.deltaTime;
    float heightVar = bird._heightAboveTarget += Mathf.Sin(Time.time * Mathf.Sin(1/2)) * .001f;

    Vector3 offset = new Vector3(
        Mathf.Cos(angle * _clockwise) * bird._circleRadius,
        heightVar,
        Mathf.Sin(angle * _clockwise) * bird._circleRadius
        );

    bird.transform.position = bird.target.transform.position + offset;
    }

    private void UpdateRotation()
    {
        Vector3 direction = (bird.target.transform.position - bird.transform.position).normalized;
        Vector3 perpendicularDir = Vector3.Cross(direction, Vector3.up).normalized;

        if(perpendicularDir != Vector3.zero)
        {
            Quaternion birdRotation = Quaternion.LookRotation(perpendicularDir);
            bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, birdRotation, 5f * Time.deltaTime);
        }
    }
}
