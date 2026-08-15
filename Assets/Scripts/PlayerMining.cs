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
    public Vector3 pickaxeBaseLocalPosition;

    public List<PickaxeLootTable> lootTables = new List<PickaxeLootTable>
    {
        new PickaxeLootTable
        {
            level = PickaxeLevel.Base,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 1, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 1, chancePercent = 50f  },
                new ResourceDrop { type = ResourceType.Gold,    amount = 1, chancePercent = 10f  },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 2f   },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Iron,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 2, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 1, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 1, chancePercent = 30f  },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 10f  },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Gold,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 3, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 2, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 1, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Diamond, amount = 1, chancePercent = 20f  },
            }
        },
        new PickaxeLootTable
        {
            level = PickaxeLevel.Diamond,
            drops = new List<ResourceDrop>
            {
                new ResourceDrop { type = ResourceType.Copper,  amount = 4, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Iron,    amount = 3, chancePercent = 100f },
                new ResourceDrop { type = ResourceType.Gold,    amount = 2, chancePercent = 100f },
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
    }

    private void Update()
    {
        pickaxeAnimator.SetBool("IsMining", isSwinging);
        UpdatePickaxeFacing();
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
        bool facingLeft = playerMovement.FacingX < 0f;

        Transform pickaxeTransform = pickaxeAnimator.transform;

        Vector3 scale = pickaxeTransform.localScale;
        scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        pickaxeTransform.localScale = scale;

        Vector3 pos = pickaxeBaseLocalPosition;
        pos.x = facingLeft ? -Mathf.Abs(pickaxeBaseLocalPosition.x) : Mathf.Abs(pickaxeBaseLocalPosition.x);
        pickaxeTransform.localPosition = pos;
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
}
