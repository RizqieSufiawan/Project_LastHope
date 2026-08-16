using UnityEngine;

[RequireComponent(typeof(Health))]
public class TurretController : MonoBehaviour
{
    public float range = 4f;
    public float fireInterval = 1f;
    public int damage = 8;

    [Header("Visual (optional)")]
    public SpriteRenderer laserSprite; // drag child GameObject's SpriteRenderer here
    public float laserDuration = 0.1f;
    public float laserWidth = 0.1f;

    private float fireTimer;
    private float laserTimer;

    private void Awake()
    {
        if (laserSprite != null) laserSprite.enabled = false;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            TryFire();
        }

        if (laserSprite != null && laserTimer > 0f)
        {
            laserTimer -= Time.deltaTime;
            if (laserTimer <= 0f) laserSprite.enabled = false;
        }
    }

    private void TryFire()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }

        if (closest == null) return;

        var damageable = closest.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage);
        }

        ShowLaser(closest.position);
    }

    private void ShowLaser(Vector3 targetPos)
    {
        if (laserSprite == null) return;

        Vector3 start = transform.position;
        Vector3 direction = targetPos - start;
        float distance = direction.magnitude;

        laserSprite.transform.position = start + direction / 2f;
        laserSprite.transform.right = direction.normalized;
        laserSprite.transform.localScale = new Vector3(distance, laserWidth, 1f);

        laserSprite.enabled = true;
        laserTimer = laserDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}