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
        GetNewTarget();
    }

    public void OnExit()
    {
        
    }

    public void StateUpdate()
    {
        timer += Time.deltaTime;

        if(timer >= enemyAircraft.aimTime)
        {
        
            if (hasTarget && TargetIsValid())
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

    private void GetNewTarget()
    {
        if(enemySpawner.enemiesSpawned.Count <= 0)
        {
            enemyAircraft.ChangeTask();
            return;
        }
        int index = Random.Range(0, enemySpawner.enemiesSpawned.Count);
        targetToFireAt = enemySpawner.enemiesSpawned[index];
        
        if(targetToFireAt == null)
        {
            hasTarget = false;
            Debug.LogWarning("Tried to get target but its null");
        }
        else
        {
            hasTarget = true;
        }
    }

    private bool TargetIsValid()
    {
        if(targetToFireAt == null)
        {
            Debug.LogWarning("Target Invalid");
            return false;
        }
        if (!enemySpawner.enemiesSpawned.Contains(targetToFireAt))
        {
            Debug.LogWarning("Target Invalid");
            return false;
        }
        Debug.LogWarning("Target Valid");
        return true;
    }
}
