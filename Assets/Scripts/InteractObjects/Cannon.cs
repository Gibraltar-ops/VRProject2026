using System;
using UnityEngine;
using UnityEngine.Audio;



public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform _instantiateOrigin;
    [SerializeField] private GameObject _vfx;
    [SerializeField] private GameObject _projectile;

    void Start()
    {


    }

    public void Fire()
    {
        GameObject projectile = Instantiate(_projectile, _instantiateOrigin.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(_instantiateOrigin.forward * 1000);

    }

}
