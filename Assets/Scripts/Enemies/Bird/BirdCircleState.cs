using UnityEngine;

public class BirdCircleState : IState
{
    public BirdCircleState(EnemyBird bird)
    {
        this.bird   = bird;
    }
        
    
    [Header("Circling")]
    private float _circleRadius;
    private float _circleMaxRadius = 15;
    private float _cirecleMinRadius = 5;

    private float _circleSpeed;
    private float _circleMaxSpeed = 2f;
    private float _circleMinSpeed = .5f;

    private float _heightAboveTarget;
    private float _minHeightAboveTarget = 5;
    private float _maxHeightAboveTarget = 12.5f;

    private int _clockwise;

    private float angle;
    private EnemyBird bird;

    public void OnEntry()
    {
        _circleRadius       = Random.Range(_cirecleMinRadius, _circleMaxRadius);
        _circleSpeed        = Random.Range(_circleMinSpeed, _circleMaxSpeed);
        _heightAboveTarget  = Random.Range(_minHeightAboveTarget, _maxHeightAboveTarget);
        _clockwise          = Random.Range(1,3);
        if(_clockwise == 2)
        {
            _clockwise = -1;
        }
    }

    public void OnExit()
    {
        bird.currentRadius = _circleRadius;
    }

    public void StateUpdate()
    {
        CircleTarget();
        UpdateRotation();
    }

    private void CircleTarget()
    {
    angle += _circleSpeed * Time.deltaTime;
    float heightVar = _heightAboveTarget += Mathf.Sin(Time.time * Mathf.Sin(1/2)) * .001f;

    Vector3 offset = new Vector3(
        Mathf.Cos(angle * _clockwise) * _circleRadius,
        heightVar,
        Mathf.Sin(angle * _clockwise) * _circleRadius
        );

    bird.transform.position = Vector3.zero + offset;
    }

    private void UpdateRotation()
    {
        Vector3 direction = (Vector3.zero - bird.transform.position).normalized;
        Vector3 perpendicularDir = Vector3.Cross(direction, Vector3.up).normalized;

        if(perpendicularDir != Vector3.zero)
        {
            Quaternion birdRotation = Quaternion.LookRotation(perpendicularDir);
            bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, birdRotation, 5f * Time.deltaTime);
        }
    }
}
