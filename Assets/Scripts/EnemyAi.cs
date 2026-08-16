using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Combat")]
    public int attackDamage = 5;
    public float attackRange = 0.8f;
    public float attackInterval = 1.5f;
    public float engageBuffer = 0.2f;

    [Header("Targeting")]
    public string[] priorityTags = { "Defense", "Cart" };
    public float playerAggroRange = 3f;
    public float retargetInterval = 1f;

    private Transform currentTarget;
    private Collider2D selfCollider;
    private Collider2D targetCollider;
    private IDamageable currentTargetDamageable;
    private float attackTimer;
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private float stuckTimer;
    private float retargetTimer;
    private bool isEngaged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        retargetTimer += Time.deltaTime;

        bool needsRetarget = currentTarget == null || !IsTargetValid(currentTarget);
        bool shouldReconsider = !isEngaged && retargetTimer >= retargetInterval;

        if (needsRetarget || shouldReconsider)
        {
            retargetTimer = 0f;
            FindNewTarget();
        }

        if (currentTarget == null) return;
        float distance = GetDistanceToTarget();

        if (!isEngaged && distance <= attackRange)
        {
            isEngaged = true;
        }
        else if (isEngaged && distance > attackRange + engageBuffer)
        {
            isEngaged = false;
        }

        if (!isEngaged)
        {
            MoveTowardsTarget();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
    }

    private bool IsTargetValid(Transform target)
    {
        return target != null && target.gameObject.activeInHierarchy;
    }

    private float GetDistanceToTarget()
    {
        if (targetCollider != null && selfCollider != null)
        {
            return Physics2D.Distance(selfCollider, targetCollider).distance;
        }
        return Vector2.Distance(transform.position, currentTarget.position);
    }

    private Vector2 GetApproachPoint()
    {
        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(rb.position);
        }
        return currentTarget.position;
    }

    private void MoveTowardsTarget()
    {
        Vector2 approachPoint = GetApproachPoint();
        Vector2 direction = (approachPoint - rb.position).normalized;

        float movedDistance = Vector2.Distance(rb.position, lastPosition);
        if (movedDistance < 0.02f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }
        lastPosition = rb.position;

        if (stuckTimer > 0.3f)
        {
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            direction = (direction + perpendicular * 0.8f).normalized;
        }

        Vector2 desiredVelocity = direction * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, 8f * Time.deltaTime);
    }

    private void TryAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer < attackInterval) return;
        attackTimer = 0f;

        if (currentTargetDamageable != null)
        {
            float dist = GetDistanceToTarget();
            Debug.Log($"{gameObject.name} attacking {currentTarget.name} at distance {dist:F2}");
            currentTargetDamageable.Damage(attackDamage);
        }
    }

    private void FindNewTarget()
    {
        Transform bestTarget = null;
        float bestDistance = Mathf.Infinity;

        foreach (var tag in priorityTags)
        {
            GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
            foreach (var candidate in candidates)
            {
                float dist = Vector2.Distance(transform.position, candidate.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestTarget = candidate.transform;
                }
            }
        }

        bool priorityInRange = bestTarget != null && bestDistance <= attackRange + 1f;

        if (!priorityInRange)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distToPlayer <= playerAggroRange)
                {
                    bestTarget = player.transform;
                }
            }
        }

        SetTarget(bestTarget);
    }

    private void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;

        if (newTarget != null)
        {
            targetCollider = newTarget.GetComponent<Collider2D>();
            currentTargetDamageable = newTarget.GetComponent<IDamageable>();
        }
        else
        {
            targetCollider = null;
            currentTargetDamageable = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerAggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}