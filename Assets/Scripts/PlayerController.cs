using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{
    private Health health;
    private PlayerMovement playerMovement;
    private PlayerMining playerMining;
    private Rigidbody2D rb;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        health = GetComponent<Health>();
        playerMovement = GetComponent<PlayerMovement>();
        playerMining = GetComponent<PlayerMining>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log("Player died — game over!");

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerMining != null) playerMining.enabled = false;
        if (FailMenuUI.Instance != null)
        {
            FailMenuUI.Instance.Show("You Died!");
        }

    }
}