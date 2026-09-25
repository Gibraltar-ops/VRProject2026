using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    private float _altitude = GameManager.instance.Altitude;

    [Header("Enemy PreFabs")]
    public EnemyParentClass bird;


    void Update()
    {
        HandleEnemySpawn();
    }

    private void HandleEnemySpawn()
    {
        switch (_altitude)
        {
            case > 10000:
                break;
            
            case > 7500:
                break;

            case > 5000:
                break;
            
            case > 2500:
                break;
            
            case >= 0:
                break;
            

        }
    }

    private void SpawnEnemy(EnemyParentClass prefab)
    {
        //Instantiate(prefab, );
    }
}
