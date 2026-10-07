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

    
    public void OnEntry()
    {
        timer = 0;
        int index = Random.Range(0, enemySpawner.enemiesSpawned.Count);
        targetToFireAt = enemySpawner.enemiesSpawned[index];
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        timer += Time.deltaTime;
        if(timer >= enemyAircraft.aimTime)
        {
            float hit = Random.Range(0f,1f);
            if(hit <= enemyAircraft.accuracy)
            {
                //targetToFireAt.KillEnemy();
                Health targetHealth = targetToFireAt.gameObject.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(1);
                }
                else
                {
                    Debug.LogWarning("Missing target health");
                }
                enemyAircraft.ChangeTask();
            }
            else
            {
                timer = 0;
            }
        }
    }
}
