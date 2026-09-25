using UnityEngine;

public class CatcherNet : MonoBehaviour
{
    public GameObject BirdCatch;
    public Collider _collider;
    public float _showBirdTimer = 10f;
    public float ShowBirdDuration = 2f;

    void Start()
    {
        
    }

    void Update()
    {
        _showBirdTimer += Time.deltaTime;
        if(_showBirdTimer > ShowBirdDuration && BirdCatch.activeInHierarchy)
        {
            BirdCatch.SetActive(false);
        }

    }

    public void ShowBirdInNet()
    {
        BirdCatch.SetActive(true);
        _showBirdTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        
    }
}
