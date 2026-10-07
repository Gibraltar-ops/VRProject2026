using UnityEngine;

public class EnemyParentClass : MonoBehaviour
{
    public BalloonTree _targetBalloonTree;
    public BalloonInfo target;

    public EnemySpawner spawnerForThis;

    virtual public void Awake()
    {
        
    }

    virtual public void Start()
    {
    }

    virtual public void Update()
    {
    }

    public void SpawnBird(EnemyParentClass enemyToSpawn, EnemySpawner enemySpawner, BalloonTree targetTree)
    {
        spawnerForThis = enemySpawner;
        spawnerForThis.enemiesSpawned.Add(this);
        EnemyParentClass newBird = Instantiate(enemyToSpawn, enemySpawner.transform.position - Vector3.down * 5, Quaternion.identity);
        newBird._targetBalloonTree = targetTree;
    }

    public void KillEnemy()
    {
        spawnerForThis.enemiesSpawned.Remove(this);
    }


}
