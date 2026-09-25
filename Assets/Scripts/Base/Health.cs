using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public event Action Die; // event f�r n�r vi d�r, animation, particlar och statemachines kan lyssna p�

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) 
        {
            return; // Already dead!
        }
        currentHealth -= damageAmount;
        if (currentHealth <= 0) 
        {
            Die?.Invoke();
        }
    }
}
