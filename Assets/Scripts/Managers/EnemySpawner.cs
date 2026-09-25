using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    private float _altitude = GameManager.instance.Altitude;

    [Header("Enemy PreFabs")]
    public GameObject bird;

    void Awake()
    {
       
    }
}
