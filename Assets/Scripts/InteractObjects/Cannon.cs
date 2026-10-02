using System;
using UnityEngine;
using UnityEngine.Audio;



public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform _instantiateOrigin;
    [SerializeField] private GameObject _vfx;
    [SerializeField] private GameObject _projectile;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _pitchVariation = 0.1f;

    void Start()
    {


    }

    public void Fire()
    {
        GameObject projectile = Instantiate(_projectile, _instantiateOrigin.position, Quaternion.LookRotation(_instantiateOrigin.forward) );
        
        projectile.GetComponent<Rigidbody>().AddForce(_instantiateOrigin.forward * 1000);

        if (_audioSource != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
            _audioSource.Play();
        }
    }

}
