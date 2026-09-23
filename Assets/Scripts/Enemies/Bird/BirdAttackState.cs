using UnityEngine;

public class BirdAttackState : IState
{
    public BirdAttackState(EnemyBird bird, GameObject target, float currentRadius)
    {
        this.bird = bird;
        this.target = target;
        this._circleRadius = currentRadius;
    }

    private GameObject target;
    private EnemyBird bird;
    private float _circleRadius;
    private bool _isAttacking;
    private Vector3 chargePoint;

    public void OnEntry()
    {
        _isAttacking = false;
        chargePoint = CalculateChargePoint();
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        bird.transform.position = Vector3.MoveTowards(bird.transform.position, chargePoint, DistanceToChargePoint()*Time.deltaTime);
        if(DistanceToChargePoint() < 1f)
        {
            _isAttacking = true;
        }

    }

    private Vector3 CalculateChargePoint()
    {
        Vector3 chargeDir = target.transform.position.normalized;
        Vector3 chargePoint = chargeDir * (_circleRadius*.5f);
        return chargePoint;
    }

    private float DistanceToChargePoint()
    {
        return Vector3.Distance(bird.transform.position, chargePoint);
    }

    private Vector3 CalculateAttackPoint()
    {
        return Vector3.zero;
    }
}
