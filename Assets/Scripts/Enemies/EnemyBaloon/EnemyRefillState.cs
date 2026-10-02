using UnityEngine;

public class EnemyRefillState : IState
{
    public EnemyRefillState(EnemyAircraft enemyAircraft)
    {
        this.enemyAircraft = enemyAircraft;
    }

    EnemyAircraft enemyAircraft;
    private float timer;

    public void OnEntry()
    {
        timer = 0;
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        if(enemyAircraft.ownBalloonTree.ActiveBalloons >= 24)
        {
            enemyAircraft.ChangeTask();
        }
        
        timer += Time.deltaTime;
        if(timer >= enemyAircraft.timeToFillBalloon)
        {
            enemyAircraft.ownBalloonTree.AddBalloon();
            enemyAircraft.ChangeTask();
        }
    }
}
