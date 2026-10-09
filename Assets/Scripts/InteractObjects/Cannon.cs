using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform _instantiateOrigin;
    [SerializeField] private ParticleSystem _vfx;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _projectileForce = 1500f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _pitchVariation = 0.1f;

    public TMPro.TextMeshPro _debugText;

    public AmmoLoader ammoLoader;

    void Start()
    {

    }

    public bool CheckAmmo()
    {
        if(ammoLoader.GetAmmo())
        {
            _debugText.text = "Has Ammo";
            return true;
        }
        else
        {
            _debugText.text = "No Ammo";
            return false;
        }
    }

    public void AmmoAdd()
    {
        _debugText.text = "Ammo ADD";
    }

    public void AmmoRemove()
    {
        _debugText.text = "Ammo REMOVE";
    }

    public void Fire()
    {
        if (CheckAmmo() == false) // return no ammo
        {
            return;
        }

        _vfx.Play();

        GameObject projectile = Instantiate(_projectile, _instantiateOrigin.position, Quaternion.LookRotation(_instantiateOrigin.forward) );
        
        projectile.GetComponent<Rigidbody>().AddForce(_instantiateOrigin.forward * _projectileForce);

        if (_audioSource != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
            _audioSource.Play();
        }
    }

}
