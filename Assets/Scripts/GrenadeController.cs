using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [Header("Throw")]
    [Tooltip("How long the grenade takes to travel from the player to the target.")]
    public float flightTime = 0.35f;
    [Tooltip("Simple visual arc/hop height while flying. Set to 0 for a flat straight-line throw.")]
    public float arcHeight = 0.3f;

    [Header("Fuse")]
    [Tooltip("Time after LANDING before it explodes.")]
    public float fuseTime = 0.6f;

    [Header("Explosion")]
    public float explosionRadius = 2f;
    public int explosionDamage = 15;

    [Header("Visual (optional)")]
    public GameObject explosionVfxPrefab;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float flightTimer;
    private bool isFlying;
    private float fuseTimer;
    private bool hasExploded;

    // Called right after Instantiate (by GrenadeThrower) to kick off the throw.
    public void Launch(Vector3 from, Vector3 to)
    {
        startPos = from;
        targetPos = to;
        transform.position = from;
        flightTimer = 0f;
        isFlying = true;
    }

    private void Update()
    {
        if (hasExploded) return;

        if (isFlying)
        {
            flightTimer += Time.deltaTime;
            float t = flightTime > 0f ? Mathf.Clamp01(flightTimer / flightTime) : 1f;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight; // little hop, purely visual
            transform.position = pos;

            if (t >= 1f)
            {
                isFlying = false;
                fuseTimer = 0f;
            }
            return;
        }

        fuseTimer += Time.deltaTime;
        if (fuseTimer >= fuseTime)
        {
            Explode();
        }
    }
    private void Explode()
    {
        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Cart")) continue;

            bool isValidTarget = hit.CompareTag("Enemy") || hit.CompareTag("Player");
            if (!isValidTarget) continue;

            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(explosionDamage);
            }
        }

        if (explosionVfxPrefab != null)
        {
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"Grenade exploded at {transform.position}, radius {explosionRadius}");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}