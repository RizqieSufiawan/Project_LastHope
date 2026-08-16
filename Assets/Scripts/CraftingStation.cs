using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftCost
{
    public ResourceType type;
    public int amount;
}

public class CraftingStation : MonoBehaviour, IInteractable
{
    public static int ActiveCount { get; private set; }

    public List<CraftCost> costs = new List<CraftCost>
    {
        new CraftCost { type = ResourceType.Copper, amount = 5 },
        new CraftCost { type = ResourceType.Iron, amount = 3 },
        new CraftCost { type = ResourceType.Gold, amount = 1 },
    };

    private void Awake()
    {
        ActiveCount++;
    }

    private void OnDestroy()
    {
        ActiveCount--;
    }

    public bool CanInteract(GameObject player)
    {
        foreach (var cost in costs)
        {
            if (ResourceManager.Instance.GetAmount(cost.type.ToString()) < cost.amount)
            {
                return false;
            }
        }
        return true;
    }

    public void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;

        foreach (var cost in costs)
        {
            ResourceManager.Instance.Spend(cost.type.ToString(), cost.amount);
        }

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddC4(1);
            Debug.Log("Crafted 1 C4!");
        }
    }

    public string GetPrompt()
    {
        return CanInteract(null) ? "Press E to Craft C4" : "Not enough resources";
    }
}