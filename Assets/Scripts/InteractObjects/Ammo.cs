using UnityEngine;


public class Ammo : MonoBehaviour
{

    public Collider colliderAmmo;
    public Transform attachedTransform;


    public bool _isAttachedToGun = false;

    public void AddToGun(Transform attachPoint)
    {
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
        if(_isAttachedToGun)
        {
            gameObject.transform.position = attachedTransform.position;
            gameObject.transform.Rotate(Vector3.right, 360f * Time.deltaTime);
        }
    }
}
