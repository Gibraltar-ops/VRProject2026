using UnityEngine;

public class EnemyFireAtBird : IState
{
    public EnemyFireAtBird(EnemyAircraft enemyAircraft, EnemySpawner enemySpawner)
    {
        this.enemyAircraft = enemyAircraft;
        this.enemySpawner = enemySpawner;
    }

    private EnemySpawner enemySpawner;
    private EnemyAircraft enemyAircraft;
    private EnemyParentClass targetToFireAt;
    private float timer;
    private bool hasTarget;

    
    public void OnEntry()
    {
        timer = 0;
        int index = Random.Range(0, enemySpawner.enemiesSpawned.Count);
        if(enemySpawner.enemiesSpawned.Count > 0)
        {
            targetToFireAt = enemySpawner.enemiesSpawned[index];
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        timer += Time.deltaTime;

        if(timer >= enemyAircraft.aimTime)
        {
        
            if (hasTarget)
            {
                float hit = Random.Range(0f,1f);
                if(hit <= enemyAircraft.accuracy)
                {
                    Health targetHealth = targetToFireAt.gameObject.GetComponent<Health>();
                    Debug.Log(targetHealth);
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(1);
                        enemyAircraft.ChangeTask();
                    }
                    else
                    {
                        Debug.LogWarning("Missing target health");
                    }
                }
                else
                {
                    timer = 0;
                }
            }
            else
            {
                Debug.Log("Found no target");
                enemyAircraft.ChangeTask();
            }
        }
    }
}
