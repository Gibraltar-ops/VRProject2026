using System;
using UnityEngine;

public class ProjectileBird : MonoBehaviour
{
    private float _timer = 0;
    public float _lifetime = 10f;

    public static event Action<GameObject> OnCannonProjectileHit;

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_timer > _lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if(health != null )
        {
            health.TakeDamage(1);
        }

        OnCannonProjectileHit?.Invoke(collision.collider.gameObject); // ballon tree is listening to this
    }
}
