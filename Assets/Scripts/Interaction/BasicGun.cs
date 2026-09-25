using System;
using UnityEngine;

public class BasicGun : MonoBehaviour
{
    public Transform RaycastOrigin;
    public static event Action<GameObject> OnGunHit;

    public void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(RaycastOrigin.position, transform.up * -1, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            OnGunHit?.Invoke(hit.collider.gameObject);
        }
        else
        {
            Debug.DrawRay(RaycastOrigin.position, (transform.up * -1) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }
}
