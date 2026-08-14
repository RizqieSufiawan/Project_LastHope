using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    public int CurrentHealth { get; private set; }

    public event Action OnDamaged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        CurrentHealth -= amount;
        OnDamaged?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
