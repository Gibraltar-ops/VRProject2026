using UnityEngine;

public class AmmoBirdSpawner : MonoBehaviour
{

    int childcount = 0;
    int childIndex = 0;
    float spawnTimer;
    float SpawnThreshold = 5f;

    public GameObject spawnPrefab;

    void Start()
    {
        childcount = transform.childCount;
    }

    public void SpawnAmmoBird(Vector3 position)
    {
        //RaycastHit hit;
        if(Physics.Raycast(position + Vector3.up*2, Vector3.down, 4f))
        {
            return;
        }
        Instantiate(spawnPrefab,position,Quaternion.LookRotation(Vector3.up-position) );
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer > SpawnThreshold) 
        {
            spawnTimer = 0;
            
            childIndex++;
            childIndex = childIndex % childcount;
            Vector3 spawnPos = transform.GetChild(childIndex).position;
            SpawnAmmoBird(spawnPos);
        }
    }
}
