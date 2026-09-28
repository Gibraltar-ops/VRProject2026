using UnityEngine;

public class EnemyFireAtPlayerState : IState
{
    public EnemyFireAtPlayerState(EnemyAircraft enemyAircraft)
    {
        this.enemyAircraft = enemyAircraft;
    }

    EnemyAircraft enemyAircraft;
    BalloonInfo targetBalloon;

    private float timer;


    public void OnEntry()
    {
        timer = 0;
        targetBalloon = GameManager.instance.PlayerBaloonTree.GetClosestAvailableBalloon(enemyAircraft.transform.position);
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        AimAndFire();
    }

    private void AimAndFire()
    {
        timer += Time.deltaTime;
        if(timer >= enemyAircraft.aimTime)
        {
            float hit = Random.Range(0f,1f);
            if(hit <= enemyAircraft.accuracy)
            {
                GameManager.instance.PlayerBaloonTree.PopBalloon(targetBalloon);
                enemyAircraft.ChangeTask();
            }
            else
            {
                timer = 0;
            }
        }
        
    }
}
