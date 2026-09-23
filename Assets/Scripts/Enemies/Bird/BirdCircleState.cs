using UnityEngine;
using UnityEngine.PlayerLoop;

public class BirdCircleState : IState
{
    public BirdCircleState(GameObject target, EnemyBird bird)
    {
        this.target = target;
        this.bird   = bird;
    }
        
    
    [Header("Circling")]
    public float _circleRadius = 5;
    public float _circleSpeed = .5f;
    public float _heightAboveTarget = 10;

    private float angle;
    private EnemyBird bird;
    private GameObject target;




    public void OnEntry()
    {
        Debug.Log("Entered Circle State");
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void StateUpdate()
    {
        CircleTarget();
        UpdateRotation();
    }

    private void CircleTarget()
    {
    angle += _circleSpeed * Time.deltaTime;

    Vector3 offset = new Vector3(
        Mathf.Cos(angle) * _circleRadius,
        _heightAboveTarget,
        Mathf.Sin(angle) * _circleRadius
        );

    bird.transform.position = target.transform.position + offset;
    }

    private void UpdateRotation()
    {
        Vector3 direction = (target.transform.position - bird.transform.position).normalized;
        Vector3 perpendicularDir = Vector3.Cross(direction, Vector3.up).normalized;

        if(perpendicularDir != Vector3.zero)
        {
            Quaternion birdRotation = Quaternion.LookRotation(perpendicularDir);
            bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, birdRotation, 5f * Time.deltaTime);
        }
    }
}
