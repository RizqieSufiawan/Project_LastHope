using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab;

    [Tooltip("Minimum time between throws, prevents spamming clicks.")]
    public float throwCooldown = 0.3f;

    [Header("Audio")]
    public AudioClip throwClip;

    private PlayerInventory inventory;
    private float cooldownTimer;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (cooldownTimer > 0f) return;

        if (HotbarUI.Instance == null || HotbarUI.Instance.GetSelectedItemId() != "Grenade") return;
        if (inventory == null || inventory.grenadeCount <= 0) return;

        ThrowGrenade();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }

    private void ThrowGrenade()
    {
        if (grenadePrefab == null)
        {
            return;
        }

        inventory.SpendGrenade(1);
        cooldownTimer = throwCooldown;

        Vector3 targetPos = GetMouseWorldPosition();
        Vector3 startPos = transform.position;

        AudioManager.Instance?.PlaySFX(throwClip);

        GameObject grenadeObj = Instantiate(grenadePrefab, startPos, Quaternion.identity);
        var controller = grenadeObj.GetComponent<GrenadeController>();
        if (controller != null)
        {
            controller.Launch(startPos, targetPos);
        }

    }
}