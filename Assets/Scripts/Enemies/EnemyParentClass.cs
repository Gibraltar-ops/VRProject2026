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

    public void SpawnEnemy(EnemyParentClass enemyToSpawn, EnemySpawner enemySpawner, BalloonTree targetTree)
    {
        spawnerForThis = enemySpawner;
        spawnerForThis.enemiesSpawned.Add(this);
        Instantiate(enemyToSpawn, enemySpawner.transform.position - Vector3.down * 5, Quaternion.identity);
        _targetBalloonTree = targetTree;
    }

    public void KillEnemy()
    {
        spawnerForThis.enemiesSpawned.Remove(this);
        Destroy(gameObject);
    }


}
