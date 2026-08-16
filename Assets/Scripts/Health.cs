using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    public int CurrentHealth { get; private set; }
    public bool destroyOnDeath = true;

    public event Action OnDamaged;
    public event Action OnDeath;

    private bool isDead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        if (isDead) return;

        CurrentHealth -= amount;
        OnDamaged?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void SetMaxHealth(int newMax, bool healToFull = true)
    {
        maxHealth = newMax;
        if (healToFull)
        {
            CurrentHealth = maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        isDead = true;
        OnDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
    public void Heal(int amount)
    {
        if (isDead) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
    }

}