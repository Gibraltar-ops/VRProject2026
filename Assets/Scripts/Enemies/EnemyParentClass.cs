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

    public void KillEnemy()
    {
        spawnerForThis.enemiesSpawned.Remove(this);
    }


}
