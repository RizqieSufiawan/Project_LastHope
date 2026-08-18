using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ResourceDrop
{
    public ResourceType type;
    public int amount = 1;
    [Range(0f, 100f)] public float chancePercent = 100f;
}

[System.Serializable]
public class PickaxeLootTable
{
    public PickaxeLevel level;
    public List<ResourceDrop> drops = new List<ResourceDrop>();
}


[RequireComponent(typeof(PlayerMovement))]
public class PlayerMining : MonoBehaviour
{
    public float mineInterval = 3f;
    public PickaxeLevel currentPickaxeLevel = PickaxeLevel.Base;
    public Animator pickaxeAnimator;
    public Sprite baseSprite;
    public Sprite ironSprite;
    public Sprite goldSprite;
    public Sprite diamondSprite;
    public Transform pickaxePivot;
    public Vector3 pickaxeBaseLocalPosition;

    [Header("Combat")]
    public int swingDamage = 10;
    public float attackRange = 1.2f;
    [Range(0f, 180f)] public float attackAngle = 60f;

    [Header("Audio")]
    public AudioClip swingClip;

    [Header("Equip")]
    public bool isEquipped = false;

    private float attackTimer;
    public List<PickaxeLootTable> lootTables = new List<PickaxeLootTable>
    {
        new PickaxeLootTable
        {
            level = PickaxeLevel.Base,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 2, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 1, chancePercent = 75f  },
                new ResourceDrop { type = ResourceType.Gold,    amount = 1, chancePercent = 15f  },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 2f   },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Iron,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 3, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 2, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 1, chancePercent = 45f  },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 12f  },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Gold,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 4, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 3, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 2, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 25f  },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Diamond,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 5, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 4, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 3, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 100f },
            }
        },
    };

    private PlayerMovement playerMovement;
    private Health health;
    private ResourceNode currentNode;

    private bool isSwinging;
    private bool isMining;
    private float mineTimer;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
        UpdatePickaxeVisual();

        var spriteRenderer = pickaxeAnimator.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isEquipped;
        }
    }

    private void OnEnable()
    {
        if (health != null) health.OnDamaged += CancelMining;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDamaged -= CancelMining;
    }

    public void OnMineInput(InputAction.CallbackContext context)
    {
        if (!isEquipped) return;

        if (context.started)
        {
            isSwinging = true;
            TryStartMining();
        }
        else if (context.canceled)
        {
            isSwinging = false;
            CancelMining();
        }
    }

    private void TryStartMining()
    {
        if (currentNode == null) return;
        isMining = true;
        mineTimer = 0f;
    }

    private void CancelMining()
    {
        isMining = false;
        mineTimer = 0f;
        MiningProgressBarUI.Instance?.Hide();
    }

    private void Update()
    {
        UpdatePickaxeFacing();

        if (!isEquipped)
        {
            pickaxeAnimator.SetBool("IsMining", false);
            return;
        }

        pickaxeAnimator.SetBool("IsMining", isSwinging);

        if (!isMining) return;

        if (playerMovement.MoveInput.sqrMagnitude > 0.01f)
        {
            CancelMining();
            return;
        }

        if (currentNode == null || currentNode.IsDepleted)
        {
            CancelMining();
            return;
        }

        mineTimer += Time.deltaTime;
        MiningProgressBarUI.Instance?.UpdateProgress(mineTimer, mineInterval);
        if (mineTimer >= mineInterval)
        {
            mineTimer = 0f;
            GrantLoot();
            bool stillActive = currentNode.ConsumeCharge();
            if (!stillActive) CancelMining();
        }
    }

    private void GrantLoot()
    {
        var table = lootTables.Find(t => t.level == currentPickaxeLevel);
        if (table == null) return;

        foreach (var drop in table.drops)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= drop.chancePercent)
            {
                ResourceManager.Instance.Add(drop.type.ToString(), drop.amount);
            }
        }
    }
    private void UpdatePickaxeVisual()
    {
        var spriteRenderer = pickaxeAnimator.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        switch (currentPickaxeLevel)
        {
            case PickaxeLevel.Base:
                spriteRenderer.sprite = baseSprite;
                break;
            case PickaxeLevel.Iron:
                spriteRenderer.sprite = ironSprite;
                break;
            case PickaxeLevel.Gold:
                spriteRenderer.sprite = goldSprite;
                break;
            case PickaxeLevel.Diamond:
                spriteRenderer.sprite = diamondSprite;
                break;
        }
    }

    private void UpdatePickaxeFacing()
    {
        bool facingLeft = playerMovement.FacingX >= 0f;

        Vector3 scale = pickaxePivot.localScale;
        scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        pickaxePivot.localScale = scale;

        Vector3 pos = pickaxeBaseLocalPosition;
        pos.x = facingLeft ? -Mathf.Abs(pickaxeBaseLocalPosition.x) : Mathf.Abs(pickaxeBaseLocalPosition.x);
        pickaxePivot.localPosition = pos;
    }
    public void ToggleEquip()
    {
        isEquipped = !isEquipped;

        var spriteRenderer = pickaxeAnimator.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isEquipped;
        }

        if (!isEquipped)
        {
            isSwinging = false;
            CancelMining();
        }

    }

    public void PerformSwingHit()
    {
        if (!isEquipped) return;

        AudioManager.Instance?.PlaySFX(swingClip);

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 aimDirection = (mouseWorldPos - (Vector2)transform.position).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Cart")) continue;
            if (!hit.CompareTag("Enemy")) continue;

            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(aimDirection, toTarget);

            if (angle > attackAngle) continue;

            var damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(swingDamage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var node = other.GetComponent<ResourceNode>();
        if (node != null) currentNode = node;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var node = other.GetComponent<ResourceNode>();
        if (node == currentNode)
        {
            currentNode = null;
            CancelMining();
        }
    }

    public void UpgradePickaxe(PickaxeLevel newLevel)
    {
        currentPickaxeLevel = newLevel;
        UpdatePickaxeVisual();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (Application.isPlaying && Camera.main != null)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 aimDirection = (mouseWorldPos - (Vector2)transform.position).normalized;

            Vector2 leftBoundary = Quaternion.Euler(0, 0, attackAngle) * aimDirection;
            Vector2 rightBoundary = Quaternion.Euler(0, 0, -attackAngle) * aimDirection;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftBoundary * attackRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightBoundary * attackRange);
        }
    }
}