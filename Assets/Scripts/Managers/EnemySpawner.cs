using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Information")]
    public BalloonTree treeToTarget;
    private float _altitude;
    private float WinAltitude;

    public List<EnemyParentClass> enemiesSpawned = new List<EnemyParentClass>();

    [Header("Enemy PreFabs")]
    public EnemyParentClass bird;

    private float timer;

    void Awake()
    {
    }

    void Start()
    {
        _altitude = GameManager.instance.Altitude;
        WinAltitude = GameManager.instance.WinAltitude;
        timer = 0;
    }

    void Update()
    {
        HandleEnemySpawn();
        timer += Time.deltaTime;
    }

    private void HandleEnemySpawn()
    {
        //float progress = _altitude / WinAltitude;
        float progress = .4f;
        switch (progress)
        {
            case > .667f:
                SpawnEnemy(bird, 3);
                break;
            
            case > .5f:
                SpawnEnemy(bird, 5);
                break;

            case > .33f:
                SpawnEnemy(bird, 10);
                break;
            
            case > .10f:
                SpawnEnemy(bird, 12.5f);
                break;
            
            case >= 0.001f:
                SpawnEnemy(bird, 15);
                break;
        }
    }

    private void SpawnEnemy(EnemyParentClass prefab, float frequency)
    {
        if(timer >= frequency)
        {
            Debug.Log("Ran Spawned enemy function");
            prefab.SpawnBird(prefab, this, treeToTarget);
            timer = 0;
        }
    }
}
