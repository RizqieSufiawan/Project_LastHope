using System;
using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour, IInteractable
{
    public float detonationDelay = 90f; // 1 menit 30 detik

    public bool IsDestroyed { get; private set; }
    public bool IsC4Placed { get; private set; }
    public event Action OnGateDestroyed;
    public event Action OnC4Placed;

    public bool CanInteract(GameObject player)
    {
        if (IsDestroyed || IsC4Placed) return false;
        var inventory = player.GetComponent<PlayerInventory>();
        return inventory != null && inventory.c4Count > 0;
    }

    public void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;

        var inventory = player.GetComponent<PlayerInventory>();
        inventory.SpendC4(1);

        IsC4Placed = true;
        OnC4Placed?.Invoke();
        Debug.Log($"C4 placed! Detonating in {detonationDelay} seconds...");

        StartCoroutine(DetonateAfterDelay());
    }

    private IEnumerator DetonateAfterDelay()
    {
        yield return new WaitForSeconds(detonationDelay);

        IsDestroyed = true;
        OnGateDestroyed?.Invoke();
        Debug.Log("C4 exploded — Gate destroyed!");

        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.enabled = false;
    }

    public string GetPrompt()
    {
        if (IsDestroyed) return "";
        if (IsC4Placed) return "C4 armed — waiting to detonate";
        return "Press E to place C4";
    }
}