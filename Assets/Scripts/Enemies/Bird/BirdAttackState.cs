using UnityEngine;
public class BirdAttackState : IState{
    public BirdAttackState(EnemyBird bird, BalloonInfo target)
    {
        this.bird = bird;
        this.target = target;
    }
    
    private enum AttackPhase
    { 
        MoveToTarget,
        Hover,
        Dive
    }

    private EnemyBird bird;
    private BalloonInfo target;
    private AttackPhase phase;

    // Attack positioning 
    private Vector3 hoverPos;
    private Vector3 attackDir; 

    // Movement
    private Vector3 attackVelocity;
    private float moveSpeed;

    private float hoverTime = 2.5f;
    private float hoverTimer;
    
    public void OnEntry()
    { 
        bird.hasAttacked = false;
        hoverTimer = hoverTime; 

        Vector3 fromTarget = (bird.transform.position - target.transform.position).normalized;

        hoverPos = target.transform.position + fromTarget * (bird._circleRadius * 0.5f); 
        attackDir = (target.transform.position - hoverPos).normalized;

        Vector3 tangent = Vector3.Cross( Vector3.up, fromTarget ) * bird.circleDirection;
        attackVelocity = tangent.normalized; 

        moveSpeed = bird._circleSpeed * 3f;

        phase = AttackPhase.MoveToTarget;
    }
        
    public void OnExit() { } 
    
    public void StateUpdate(){ 
        switch (phase){ 
            case AttackPhase.MoveToTarget: 
                MoveToTarget();
                break;
            
            case AttackPhase.Hover:
                Hover();
                break;
                
            case AttackPhase.Dive:
                Dive();
                break;
        } }
        
    private void MoveToTarget()
    {
        Vector3 toHover = hoverPos - bird.transform.position;
        float distanceToHover = toHover.magnitude;

        if(distanceToHover > 0.01f)
            { 
                Vector3 desiredDirection = toHover.normalized;
                    attackVelocity = Vector3.Slerp( attackVelocity, desiredDirection, 2f * Time.deltaTime ).normalized;
            }

        bird.transform.position += attackVelocity * moveSpeed * Time.deltaTime;
        if(attackVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(attackVelocity);
            bird.transform.rotation = Quaternion.Slerp( bird.transform.rotation, targetRotation, 5f * Time.deltaTime );
        }

        if (distanceToHover < 0.1f)
        { 
            bird.transform.position = hoverPos; phase = AttackPhase.Hover; 
        } 
    }
        
    private void Hover()
    {
        hoverTimer -= Time.deltaTime;
        Vector3 toTarget = target.transform.position - bird.transform.position;

        if (toTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation( toTarget.normalized );
            bird.transform.rotation = Quaternion.Slerp( bird.transform.rotation, targetRotation, 3f * Time.deltaTime );
        } 
        if (hoverTimer <= 0f)
        { 
            phase = AttackPhase.Dive;
        }
    }
        
    private void Dive()
    {
        bird.transform.position += attackDir * bird._attackingSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(attackDir);
        bird.transform.rotation = Quaternion.Slerp( bird.transform.rotation, targetRotation, 10f * Time.deltaTime ); 
        float distancePastTarget = Vector3.Dot( bird.transform.position - target.transform.position, attackDir );
        
        if (distancePastTarget >= bird._circleRadius * 0.5f)
        {
            bird._stateMachine.ChangeState( bird._birdCircleState );
        } 
    } 
}