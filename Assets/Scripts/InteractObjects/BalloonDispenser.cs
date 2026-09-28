using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class BalloonDispenser : MonoBehaviour
{
    [SerializeField] private GameObject _balloonPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _waitUntilNextSpawn = 1f;
    private DeflatedBallon _currentSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnItem(_balloonPrefab);
    }

    void SpawnItem(GameObject item)
    {
        GameObject currentBalloon = Instantiate(_balloonPrefab, _spawnPoint.position, Quaternion.identity);
        _currentSpawn = currentBalloon.GetComponent<DeflatedBallon>();
        _currentSpawn.OnPickup += StartSpawningNew;

    }

    void StartSpawningNew()
    {
        _currentSpawn.OnPickup -= StartSpawningNew;
        _currentSpawn = null;
        StartCoroutine(Wait(_waitUntilNextSpawn, () =>
        {
            SpawnItem(_balloonPrefab);
        }));
    }

    IEnumerator Wait(float waitTime, Action callback)
    {
        float seconds = waitTime;
        float timer = 0f;
        while (timer <= waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        callback.Invoke();
    }
}
