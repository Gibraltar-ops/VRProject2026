using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Information")]
    private float _altitude = GameManager.instance.Altitude;
    private float WinAltitude = GameManager.instance.WinAltitude;

    public int enemiesSpawned;

    [Header("Enemy PreFabs")]
    public EnemyParentClass bird;

    private float timer;

    void Start()
    {
        enemiesSpawned = 0;
        timer = 0;
    }

    void Update()
    {
        HandleEnemySpawn();
        timer += Time.deltaTime;
    }

    private void HandleEnemySpawn()
    {
        float progress = _altitude / WinAltitude;
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
            Instantiate(prefab, new Vector3(0,-5,0), quaternion.identity);
            enemiesSpawned++;
            timer = 0;
        }
    }
}
