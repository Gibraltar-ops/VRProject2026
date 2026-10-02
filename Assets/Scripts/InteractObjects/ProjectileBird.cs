using System;
using UnityEngine;

public class ProjectileBird : MonoBehaviour
{
    private float _timer = 0;
    public float _lifetime = 10f;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _fireSounds;
    [SerializeField] private float _pitchVariation = 0.1f;
    

    public static event Action<GameObject> OnCannonProjectileHit;
    
    private void Start()
    {
        if (_audioSource != null && _fireSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, _fireSounds.Length);
            
            _audioSource.clip = _fireSounds[randomIndex];
            _audioSource.pitch = UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
            _audioSource.Play();
        }
    }

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
            health.AddImpulse(collision.impulse);
        }

        OnCannonProjectileHit?.Invoke(collision.collider.gameObject); // ballon tree is listening to this
    }
}
