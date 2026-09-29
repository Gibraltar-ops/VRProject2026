using UnityEngine;

public class BirdCircleState : IState
{
    public BirdCircleState(EnemyBird bird){
        this.bird = bird;
        } 
    
    [Header("Circling")]
    private float _circleMaxRadius = 15f;
    private float _circleMinRadius = 5f;
    private float _circleMaxSpeed = 2f;
    private float _circleMinSpeed = 0.5f;
    private float _minHeightAboveTarget = 0f;
    private float _maxHeightAboveTarget = 4f;
    private float angle;
    
    private EnemyBird bird;
    private float timer;
    
    public void OnEntry()
    {
        if (bird._circleRadius <= 0f)
        {
            bird._circleRadius = Random.Range( _circleMinRadius, _circleMaxRadius );
            bird._circleSpeed = Random.Range( _circleMinSpeed, _circleMaxSpeed );
            bird._heightAboveTarget = Random.Range( _minHeightAboveTarget, _maxHeightAboveTarget ); 
            bird.circleDirection = Random.Range(0, 2) == 0 ? -1 : 1; }
            timer = 10f; 
    }

    public void OnExit()
    {}
    
    public void StateUpdate()
    {
        if (bird.target == null)
        {
            bird.target = bird.targetBalloonTree.GetClosestAvailableBalloon( bird.gameObject.transform.position );
            if (bird.target == null) 
            {
                bird.target = bird.targetBalloonTree .GetOutmostBalloon();
            } 
        }
        CircleTarget();
        UpdateRotation();
        timer -= Time.deltaTime;
        if (timer <= 0f)
        { /* * Make sure the attack state knows which * balloon we're attacking. */
            bird._birdAttackState = new BirdAttackState( bird, bird.target );
            bird._stateMachine.ChangeState( bird._birdAttackState );
        }
    } 

    private void CircleTarget()
    {
        angle += bird._circleSpeed * Time.deltaTime;
        float heightVar = bird._heightAboveTarget + Mathf.Sin(Time.time * Mathf.Sin(0.5f)) * 0.001f; 
        
        Vector3 offset = new Vector3(
            Mathf.Cos( angle * bird.circleDirection ) * bird._circleRadius,
            heightVar,
            Mathf.Sin( angle * bird.circleDirection ) * bird._circleRadius
            );

            bird.transform.position = bird.target.transform.position + offset;
    }

    private void UpdateRotation()
    { /* * Direction toward the target. */
        Vector3 direction = ( bird.target.transform.position - bird.transform.position ).normalized; /* * Tangent to the orbit. */
        Vector3 perpendicularDir = Vector3.Cross( direction, Vector3.up ).normalized;
        perpendicularDir *= bird.circleDirection;
        if(perpendicularDir != Vector3.zero)
        {
            Quaternion birdRotation = Quaternion.LookRotation( perpendicularDir );
            bird.transform.rotation = Quaternion.Slerp( bird.transform.rotation, birdRotation, 5f * Time.deltaTime ); 
        }
    } 
}