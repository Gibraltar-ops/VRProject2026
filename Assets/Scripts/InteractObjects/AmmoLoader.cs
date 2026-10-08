using UnityEngine;

public class AmmoLoader : MonoBehaviour
{

    public Ammo ammo;
    public Transform attachPoint;

    public bool GetAmmo()
    {
        if (ammo != null)
        {
            ammo.RemoveAmmo();
            ammo = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ammo ammo = other.GetComponent<Ammo>();
        if (ammo == null) 
        {
            Debug.LogError("No Ammo Component found!");
            return; 
        }

        ammo.AddToGun(attachPoint);

    }
}
