using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public event Action Die; // event för när vi dör, animation, particlar och statemachines kan lyssna på

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
