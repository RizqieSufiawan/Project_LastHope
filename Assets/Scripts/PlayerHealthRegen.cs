using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerHealthRegen : MonoBehaviour
{
    [Header("Pengaturan Regen")]
    public int regenAmount = 2;

    public float regenInterval = 1f;

    public float delayAfterDamage = 3f;

    private Health health;
    private float regenTimer;
    private float delayTimer;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnDamaged += ResetDelayTimer;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDamaged -= ResetDelayTimer;
    }

    private void ResetDelayTimer()
    {
        delayTimer = delayAfterDamage;
        regenTimer = 0f;
    }

    private void Update()
    {
        if (health.CurrentHealth >= health.maxHealth) return;

        if (health.CurrentHealth <= 0) return;

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        regenTimer += Time.deltaTime;
        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            health.Heal(regenAmount);
        }
    }
}