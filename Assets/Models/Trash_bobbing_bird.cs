using UnityEngine;

public class Trash : MonoBehaviour
{
    bool _isCatched = false;
    float _randomTimeOffset;
    float _amplitude;
    float _speed;
    Vector3 _startPos;

    public GameObject particleVFX;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _randomTimeOffset = Random.value;
        _amplitude = Random.value + 1.0f;
        _startPos = transform.position;
        _speed = Random.value + 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_isCatched == false))
        {
            float yOffset = Mathf.Sin((Time.time + _randomTimeOffset) * _speed) * _amplitude;
            transform.position = new Vector3(_startPos.x, _startPos.y + yOffset, _startPos.z);
        }

    }

    public void Die()
    {
        Instantiate(particleVFX,transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CatcherNet catcherNet = collision.gameObject.GetComponent<CatcherNet>();
        if(catcherNet != null)
        {
            catcherNet.ShowBirdInNet();
            Die();
        }
    }
}
