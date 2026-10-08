using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Ammo : MonoBehaviour
{

    public Collider colliderAmmo;

    public XRGrabInteractable grabInteractable;

    public bool _isAttachedToGun = false;

    public void AddToGun()
    {
        _isAttachedToGun = true;
        colliderAmmo.enabled = false;
        grabInteractable.enabled = false;
    }

    private void Update()
    {
        if(_isAttachedToGun)
        {
            gameObject.transform.Rotate(Vector3.right, 360f * Time.deltaTime);
        }
    }
}
