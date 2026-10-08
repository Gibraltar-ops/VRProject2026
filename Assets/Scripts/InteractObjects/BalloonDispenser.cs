using System;
using System.Collections;
using System.Threading;
using UnityEngine;



public class BalloonDispenser : MonoBehaviour
{
    enum DispenserState
    {
        Ready,
        NotReady
    }
    private DispenserState _state = DispenserState.Ready;
    [SerializeField] private GameObject _balloonPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _waitUntilNextSpawn = 1f;
    private DeflatedBallon _currentSpawn;

    public bool AutoSpawn = false;

    void Start()
    {

    }

    public void SpawnItem()
    {
        if (AutoSpawn)
        {
            GameObject currentBalloon = Instantiate(_balloonPrefab, _spawnPoint.position, Quaternion.identity);
            _currentSpawn = currentBalloon.GetComponent<DeflatedBallon>();
            currentBalloon.GetComponent<Rigidbody>().isKinematic = true;

            _currentSpawn.OnPickup += StartSpawningNew;
        }
        else
        {
            if (_state == DispenserState.NotReady) return;
            GameObject currentBalloon = Instantiate(_balloonPrefab, _spawnPoint.position, Quaternion.identity);
            _currentSpawn = currentBalloon.GetComponent<DeflatedBallon>();
            currentBalloon.GetComponent<Rigidbody>().isKinematic = true;

            _state = DispenserState.NotReady;
            _currentSpawn.OnPickup += EnableSpawnDelayed;
        }

    }

    void StartSpawningNew()
    {
        _currentSpawn.OnPickup -= StartSpawningNew;
        _currentSpawn.GetComponent<Rigidbody>().isKinematic = false;
        _currentSpawn = null;
        StartCoroutine(WaitForSeconds(_waitUntilNextSpawn, () =>
        {
            SpawnItem();
        }));
    }

    void EnableSpawnDelayed()
    {
        _currentSpawn.OnPickup -= EnableSpawnDelayed;
        _currentSpawn.GetComponent<Rigidbody>().isKinematic = false;
        _currentSpawn = null;
        StartCoroutine(WaitForSeconds(_waitUntilNextSpawn, () =>
        {
            _state = DispenserState.Ready;
        }));
    }

    IEnumerator WaitForSeconds(float waitTime, Action callback)
    {
        float timer = 0f;
        while (timer <= waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        callback.Invoke();
    }
}
