using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Information")]
    public BalloonTree treeToTarget;
    [SerializeField] private int birdCap = 10;
    private float _altitude;
    private float WinAltitude;
    private bool boostActive;

    public List<EnemyParentClass> enemiesSpawned = new List<EnemyParentClass>();

    [Header("Enemy PreFabs")]
    public EnemyParentClass bird;

    private float timer;

    public bool _useDebugProgress = false;
    public float _debugProgress = 0.5f;

    void Awake()
    {
    }

    void Start()
    {
        WinAltitude = GameManager.instance.WinAltitude;
        timer = 0;
        boostActive = false;
    }

    void Update()
    {
        _altitude = GameManager.instance.Altitude;
        HandleEnemySpawn();
        timer += Time.deltaTime;
    }

    private void HandleEnemySpawn()
    {
        if(enemiesSpawned.Count >= birdCap)
        {
            return;
        }
        if (boostActive)
        {
            return;
        }
        float progress = _altitude / WinAltitude;

        if (_useDebugProgress) { progress = _debugProgress; }

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
            prefab.SpawnBird(prefab, this, treeToTarget);
            timer = 0;
        }
    }

    public void SpawnDisaster(int amountOfBirds)
    {
        for (int i = 0; i < amountOfBirds; i++)
        {
            if(enemiesSpawned.Count >= birdCap)
            {
                return;
            }
            bird.SpawnBird(bird, this, treeToTarget);
        }
    }

    public void SetBoostValue(bool value)
    {
        boostActive = value;
    }
}
