using System;
using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour, IInteractable
{
    public float detonationDelay = 90f;

    [Header("C4 Visual")]
    [Tooltip("Prefab shown on the gate once C4 is placed (before detonation).")]
    public GameObject c4VisualPrefab;
    [Tooltip("Optional — where the C4 visual spawns. Leave empty to use the Gate's own position.")]
    public Transform c4AttachPoint;
    [Tooltip("Optional — explosion VFX played at detonation. Can reuse the same ExplosionEffect prefab used by Grenade.")]
    public GameObject detonationVfxPrefab;

    [Header("Screen Flash")]
    public Color flashColor = Color.white;
    public float flashFadeDuration = 0.5f;

    public bool IsDestroyed { get; private set; }
    public bool IsC4Placed { get; private set; }
    public event Action OnGateDestroyed;
    public event Action OnC4Placed;

    private GameObject c4VisualInstance;

    public bool CanInteract(GameObject player)
    {
        if (IsDestroyed || IsC4Placed) return false;

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null || inventory.c4Count <= 0) return false;

        if (HotbarUI.Instance == null || HotbarUI.Instance.GetSelectedItemId() != "C4") return false;

        return true;
    }

    public void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;

        var inventory = player.GetComponent<PlayerInventory>();
        inventory.SpendC4(1);

        IsC4Placed = true;
        SpawnC4Visual();
        OnC4Placed?.Invoke();
        Debug.Log($"C4 placed! Detonating in {detonationDelay} seconds...");

        StartCoroutine(DetonateAfterDelay());
    }

    private void SpawnC4Visual()
    {
        if (c4VisualPrefab == null)
        {
            Debug.LogWarning("GateController: c4VisualPrefab not assigned — C4 will have no visible model.");
            return;
        }

        Vector3 spawnPos = c4AttachPoint != null ? c4AttachPoint.position : transform.position;
        Transform parent = c4AttachPoint != null ? c4AttachPoint : transform;

        c4VisualInstance = Instantiate(c4VisualPrefab, spawnPos, Quaternion.identity, parent);
    }

    private IEnumerator DetonateAfterDelay()
    {

        yield return new WaitForSeconds(detonationDelay);

        IsDestroyed = true;
        OnGateDestroyed?.Invoke();
        Debug.Log("C4 exploded — Gate destroyed!");

        if (ScreenFlash.Instance != null)
        {
            Debug.Log("ScreenFlash triggered!");
            ScreenFlash.Instance.Flash(flashColor, flashFadeDuration);
        }
        else
        {
            Debug.LogWarning("ScreenFlash.Instance is NULL!");
        }

        if (c4VisualInstance != null) Destroy(c4VisualInstance);

        if (detonationVfxPrefab != null)
        {
            Vector3 vfxPos = c4AttachPoint != null ? c4AttachPoint.position : transform.position;
            Instantiate(detonationVfxPrefab, vfxPos, Quaternion.identity);
        }
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var rend in renderers)
        {
            rend.enabled = false;
        }
    }

    public string GetPrompt()
    {
        if (IsDestroyed) return "";
        if (IsC4Placed) return "C4 armed — waiting to detonate";
        if (HotbarUI.Instance != null && HotbarUI.Instance.GetSelectedItemId() != "C4") return "Select C4 in hotbar first";
        return "Press E to place C4";
    }
}