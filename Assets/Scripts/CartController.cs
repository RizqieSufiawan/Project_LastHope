using UnityEngine;

[RequireComponent(typeof(Health))]
public class CartController : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleCartDestroyed;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleCartDestroyed;
    }

    private void HandleCartDestroyed()
    {
        Debug.Log("Cart destroyed — level failed!");
        // TODO: nanti dihubungkan ke sistem game-over/UI pas sistem itu udah dibangun
    }
}