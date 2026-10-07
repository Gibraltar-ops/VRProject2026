using System;
using UnityEngine;
using UnityEngine.Audio;



public class BasicGun : MonoBehaviour
{
    public Transform RaycastOrigin;
    public static event Action<GameObject> OnGunHit;
    public bool _debug = false;
    public GameObject vfx;

    public LineRenderer _lineRenderObject;
    public GameObject _debug_Sphere;

    void Start()
    {
        if (_debug)
        {
            _lineRenderObject.gameObject.SetActive(true);
            _debug_Sphere.gameObject.SetActive(true);
        }
        else
        {
            _lineRenderObject.gameObject.SetActive(false);
            _debug_Sphere.gameObject.SetActive(false);
        }
     
    }

    public void SetKnots(Vector3 start, Vector3 end)
    {
        _lineRenderObject.positionCount = 2;
        _lineRenderObject.SetPosition(0, start);
        _lineRenderObject.SetPosition(1, end);
    }
    
    public void Fire()
    {
        
        Instantiate(vfx, RaycastOrigin.position, Quaternion.LookRotation(transform.up * -1, transform.right * -1));

        RaycastHit hit;
        if (Physics.Raycast(RaycastOrigin.position, transform.up * -1, out hit, Mathf.Infinity))
        {
            if (_debug)
            {
                SetKnots(RaycastOrigin.position, RaycastOrigin.forward * 500f);
                _debug_Sphere.transform.position = hit.point;

                Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
            OnGunHit?.Invoke(hit.collider.gameObject); // ballon tree is listening to this

            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.AddImpulse(RaycastOrigin.transform.forward * 5f);
                health.TakeDamage(1);
            }
        }
        else
        {
            if (_debug)
            {
                SetKnots(RaycastOrigin.position, RaycastOrigin.forward * 500f);
                _debug_Sphere.transform.position = Vector3.zero;

                Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * 1000, Color.red);
                Debug.Log("Did not Hit");
            }
        }
    }
    
}
