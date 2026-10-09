using UnityEngine;

public class EnemyWait : IState
{
    public EnemyWait(EnemyAircraft enemyAircraft)
    {
        this.enemyAircraft = enemyAircraft;
    }
    private EnemyAircraft enemyAircraft;

    private float timer;
    private float timeToWait = 5;

    public void OnEntry()
    {
      timer = 0;  
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        timer += Time.deltaTime;
        if(timer > timeToWait)
        {
            enemyAircraft.ChangeTask();
        }
    }
}
