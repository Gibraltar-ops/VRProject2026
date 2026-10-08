using UnityEngine;

public class AmmoLoader : MonoBehaviour
{

    public Ammo ammo;
    public Transform attachPoint;

    public bool DebugInfiniteAmmo = false;

    public bool GetAmmo()
    {
        if (DebugInfiniteAmmo)
        {
            return true;
        }

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
            Debug.LogWarning($"No Ammo Component found!");
            return; 
        }

        ammo.AddToGun(attachPoint);
        this.ammo = ammo; // assign to local ammo
    }

}
