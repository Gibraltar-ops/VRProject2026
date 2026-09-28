using System;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class BasicGun : MonoBehaviour
{
    public Transform RaycastOrigin;
    public static event Action<GameObject> OnGunHit;
    public bool _debug = false;
    public GameObject vfx;
    
    public AudioClip gunShootSound;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    
    private AudioSource source;
    public AudioMixerGroup sfxGroup;
    
    void Start()
    {
       
        source = GetComponent<AudioSource>();
        
       
        source.playOnAwake = false;
        source.spatialBlend = 1f; 

        if (sfxGroup != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
        }
    }
    
    public void Fire()
    {
        PlayGunSFX(gunShootSound);
        
        Instantiate(vfx, RaycastOrigin.position, Quaternion.LookRotation(transform.up * -1, transform.right * -1));

        RaycastHit hit;
        if (Physics.Raycast(RaycastOrigin.position, transform.up * -1, out hit, Mathf.Infinity))
        {
            if (_debug)
            {
                Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
            OnGunHit?.Invoke(hit.collider.gameObject); // ballon tree is listening to this

            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
        else
        {
            if (_debug)
            {
                Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * 1000, Color.red);
                Debug.Log("Did not Hit");
            }
        }
    }
    
    private void PlayGunSFX(AudioClip clipToPlay)
    {
        if (clipToPlay == null)
        {
            return;
        }
        
        source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clipToPlay);
    }
}
