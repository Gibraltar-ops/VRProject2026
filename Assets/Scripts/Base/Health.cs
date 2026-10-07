using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public event Action Die; // event for nar vi dor, animation, particlar och statemachines kan lyssna pa
    public Vector3 Impulse;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        
        if (currentHealth <= 0) 
        {
            return; // Already dead!
        }
        currentHealth -= damageAmount;
        Debug.Log("Remaining health: " + currentHealth);
        if (currentHealth <= 0) 
        {
            Die?.Invoke();
        }
    }

    public void AddImpulse(Vector3 incomingImpulse)
    {
        Impulse = incomingImpulse;
    }
}
