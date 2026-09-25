using UnityEditor;
using UnityEngine;

public class BirdDeadState : IState
{
    public BirdDeadState(EnemyBird bird)
    {
        this.bird = bird;
    }

    private EnemyBird bird;
    private float fallingSpeed = 10f;


    private Vector3 gravePos;

    public void OnEntry()
    {
        //Play a sound
        gravePos = bird.transform.position - new Vector3(0,-100,0);
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        bird.transform.position += Vector3.Lerp(bird.transform.position, gravePos, fallingSpeed);
        if(Vector3.Distance(bird.transform.position, gravePos) < 5f)
        {
            bird.KillBird();
        }
    }
}
