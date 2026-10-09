using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class Ammo : MonoBehaviour
{
    public enum State
    {
        Alive,
        Ammo,
        Dead
    }
    public State state;

    public Collider colliderAmmo;
    public Transform attachedTransform;
    public XRGrabInteractable grabInteractable;

    public bool _isAttachedToGun = false;

    bool _isCatched = false;
    bool _isAlive = true;
    float _randomTimeOffset;
    float _amplitude;
    float _speed;
    Vector3 _startPos;

    public GameObject particleVFX;

    public Health health;
    public float forceMult = 10f;

    void Start()
    {
        _randomTimeOffset = Random.value;
        _amplitude = Random.value + .15f;
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
        state = State.Dead;
        _isAlive = false;
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

        Instantiate(particleVFX, transform.position, Quaternion.identity);
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CatcherNet catcherNet = collision.gameObject.GetComponent<CatcherNet>();
        if (catcherNet != null)
        {
            catcherNet.ShowBirdInNet();
            Die();
        }
    }

    public void KillByGrab()
    {
        state = State.Ammo;
    }

    public Ammo.State GetBirdStateEnum()
    {
        return state;
    }

    public void AddToGun(Transform attachPoint)
    {
        grabInteractable.enabled = false;

        _isAttachedToGun = true;
        colliderAmmo.enabled = false;
        attachedTransform = attachPoint;
    }

    public void RemoveAmmo()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (_isCatched == true) { return; }

        if (state == State.Ammo)
        {
            if (_isAttachedToGun)
            {
                gameObject.transform.position = attachedTransform.position;
                gameObject.transform.Rotate(Vector3.right, 360f * Time.deltaTime);
            }
        }
        else if (state == State.Dead) 
        {
            
        }
        else if(state == State.Alive)
        {
            float yOffset = Mathf.Sin((Time.time + _randomTimeOffset) * _speed) * _amplitude;
            transform.position = new Vector3(_startPos.x, _startPos.y + yOffset, _startPos.z);
        }
    }

}
