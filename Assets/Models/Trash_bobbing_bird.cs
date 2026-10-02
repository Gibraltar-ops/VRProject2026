using UnityEngine;

public class Trash : MonoBehaviour
{
    bool _isCatched = false;
    bool _isAlive = true;
    float _randomTimeOffset;
    float _amplitude;
    float _speed;
    Vector3 _startPos;

    public GameObject particleVFX;

    public Health health;
    public float forceMult = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _randomTimeOffset = Random.value;
        _amplitude = Random.value + 1.0f;
        _startPos = transform.position;
        _speed = Random.value + 1f;
        _isAlive = true;

        health.Die += Health_Die;
    }
    private void OnDisable()
    {
        health.Die -= Health_Die;
    }

    private void Health_Die()
    {

        Die();
    }

    // Update is called once per frame
    void Update()
    {
        if ( (_isCatched == false && _isAlive == true) )
        {
            float yOffset = Mathf.Sin((Time.time + _randomTimeOffset) * _speed) * _amplitude;
            transform.position = new Vector3(_startPos.x, _startPos.y + yOffset, _startPos.z);
        }

    }

    public void Die()
    {
        _isAlive = false;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        Debug.Log("Health impulse" + health.Impulse + "  -  forceMult " + forceMult + "  - new impulse : " + health.Impulse.normalized * forceMult);
        rb.AddForce(health.Impulse.normalized * forceMult, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.value * 2f - 1, Random.value * 2f - 1, Random.value * 2f - 1) * 2f, ForceMode.Impulse);

        Instantiate(particleVFX,transform.position, Quaternion.identity);
        //Destroy(gameObject,2f);
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
