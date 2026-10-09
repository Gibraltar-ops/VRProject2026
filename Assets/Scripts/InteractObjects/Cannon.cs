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

    public bool debugBool;
    public TMPro.TextMeshPro _debugText;

    public AmmoLoader ammoLoader;

    Vector3 _startPos;

    void Start()
    {
        _startPos = transform.position;
        if (debugBool)
        {
            _debugText.gameObject.SetActive(true);
        }
        else
        {
            _debugText.gameObject.SetActive(false);
        }
    }

    void CheckBounds()
    {
        if (transform.position.sqrMagnitude > 40f)
        {
            transform.position = _startPos;
        }
    }

    private void Update()
    {
        CheckBounds();
    }

    public bool CheckAmmo()
    {
        if(ammoLoader.GetAmmo())
        {
            if (debugBool)
            {
                _debugText.text = "Has Ammo";
            }
            return true;
        }
        else
        {
            if (debugBool)
            {
                _debugText.text = "No Ammo";
            }
            return false;
        }
    }

    public void AmmoAdd()
    {
        if (debugBool)
        {
            _debugText.text = "Ammo ADD";
        }
    }

    public void AmmoRemove()
    {
        if (debugBool)
        {
            _debugText.text = "Ammo REMOVE";
        }
    }

    public void Fire()
    {
        if (CheckAmmo() == false) // return no ammo
        {
            return;
        }

        _vfx.Play(true);

        GameObject projectile = Instantiate(_projectile, _instantiateOrigin.position, Quaternion.LookRotation(_instantiateOrigin.forward) );
        
        projectile.GetComponent<Rigidbody>().AddForce(_instantiateOrigin.forward * _projectileForce);

        if (_audioSource != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
            _audioSource.Play();
        }
    }

}
