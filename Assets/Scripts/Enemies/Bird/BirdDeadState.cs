using UnityEditor;
using UnityEngine;

public class BirdDeadState : IState
{
    public BirdDeadState(EnemyBird bird)
    {
        this.bird = bird;
    }

    private EnemyBird bird;
    //private float fallingSpeed = 10f;
    private float time = 0;

    private Vector3 gravePos;
    private Vector3 startPos;

    public void OnEntry()
    {
        bird._birdStateEnum = EnemyBird.BirdStateEnum.Dead;

        //Play a sound
        startPos = bird.transform.position;
        gravePos = bird.transform.position + new Vector3(0f, -20f, 0f);
        time = 0f;
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        time += (Time.deltaTime / 3f); // fall for 5 sek

        //bird.transform.position = Vector3.Lerp(startPos, gravePos, time);

        Rigidbody rb = bird.gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(bird._health.Impulse, ForceMode.Impulse);

        //if(Vector3.Distance(bird.transform.position, gravePos) < 5f)
        if (time >= 1)
        {
            bird.KillEnemy();
        }
    }
}
