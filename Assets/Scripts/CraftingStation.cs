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

    [Header("C4")]
    public List<CraftCost> c4Costs = new List<CraftCost>
{
    new CraftCost { type = ResourceType.Copper, amount = 10 },
    new CraftCost { type = ResourceType.Iron, amount = 5 },
    new CraftCost { type = ResourceType.Gold, amount = 3 },
    new CraftCost { type = ResourceType.Diamond, amount = 1 },
};

    [Header("Pickaxe Upgrade")]
    public List<CraftCost> upgradeToIronCosts = new List<CraftCost> { new CraftCost { type = ResourceType.Iron, amount = 3 } };
    public List<CraftCost> upgradeToGoldCosts = new List<CraftCost> { new CraftCost { type = ResourceType.Gold, amount = 3 } };
    public List<CraftCost> upgradeToDiamondCosts = new List<CraftCost> { new CraftCost { type = ResourceType.Diamond, amount = 3 } };

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
        return true;
    }

    public void Interact(GameObject player)
    {
        CraftingMenuUI.Instance.Open(this, player);
    }

    public string GetPrompt()
    {
        return "Press E to open Crafting Menu";
    }

    public List<CraftCost> GetC4Costs() => c4Costs;

    public List<CraftCost> GetPickaxeUpgradeCosts(PickaxeLevel currentLevel)
    {
        switch (currentLevel)
        {
            case PickaxeLevel.Base: return upgradeToIronCosts;
            case PickaxeLevel.Iron: return upgradeToGoldCosts;
            case PickaxeLevel.Gold: return upgradeToDiamondCosts;
            default: return null;
        }
    }
}