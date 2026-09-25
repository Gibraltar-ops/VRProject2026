using UnityEditor.ShortcutManagement;
using UnityEngine;

public class BirdAttackState : IState
{
    public BirdAttackState(EnemyBird bird, GameObject target, float currentRadius)
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

    private GameObject target;
    private EnemyBird bird;

    private AttackPhase phase;
    
    private Vector3 hoverPos;
    private Vector3 attackDir;
    private Quaternion diveRotation;

    private float hoverTime = 2.5f;
    private float hoverTimer;

    public void OnEntry()
    {
        hoverTimer = hoverTime;
        Vector3 toTarget = (bird.transform.position - target.transform.position).normalized;

        hoverPos = target.transform.position + toTarget * (bird._circleRadius) * .5f;
        attackDir = (target.transform.position - hoverPos).normalized;

        diveRotation = Quaternion.LookRotation(attackDir);
        phase = AttackPhase.MoveToTarget;
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        switch(phase)
        {
            case AttackPhase.MoveToTarget:
                MoveToTarget();
                break;
            
            case AttackPhase.Hover:
                Hover();
                break;

            case AttackPhase.Dive:
                Dive();
                break;
        }
    }

    
    private void MoveToTarget()
    {
        bird.transform.position = Vector3.MoveTowards(
            bird.transform.position,
            hoverPos,
            bird._circleSpeed * 3.14f * Time.deltaTime
        );

        bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, TargetRotation(hoverPos), Time.deltaTime);

        if(Vector3.Distance(bird.transform.position, hoverPos) < 0.1f)
        {
            phase = AttackPhase.Hover;
        }
    }

    private void Hover()
    {
        hoverTimer -= Time.deltaTime;
        bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, TargetRotation(attackDir), Time.deltaTime * 2);

        if(hoverTimer <= 0f)
        {
            phase = AttackPhase.Dive;
        }
    }

    private void Dive()
    {
        bird.transform.position += attackDir * bird._attackingSpeed * Time.deltaTime;
        float distance_past_target = Vector3.Dot(bird.transform.position - target.transform.position, attackDir);

        if(distance_past_target >= bird._circleRadius * .5f)
        {
            bird._stateMachine.ChangeState(bird._birdCircleState);
        }
    }

    private Quaternion TargetRotation(Vector3 pointToLookAt)
    {
        return Quaternion.LookRotation(pointToLookAt);
    }

}
