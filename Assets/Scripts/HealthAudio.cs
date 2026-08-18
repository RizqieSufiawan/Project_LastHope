using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthAudio : MonoBehaviour
{
    public AudioClip damagedClip;
    public AudioClip deathClip;
    [Range(0f, 1f)] public float volume = 1f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
        health.OnDeath -= HandleDeath;
    }

    private void HandleDamaged() => AudioManager.Instance?.PlaySFX(damagedClip, volume);
    private void HandleDeath() => AudioManager.Instance?.PlaySFX(deathClip, volume);
}