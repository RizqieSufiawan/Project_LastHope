using System.Collections.Generic;

public static class CraftCostUtility
{
    public static bool CanAfford(List<CraftCost> costs)
    {
        if (costs == null) return false;
        foreach (var cost in costs)
        {
            if (ResourceManager.Instance.GetAmount(cost.type.ToString()) < cost.amount)
                return false;
        }
        return true;
    }

    public static void Spend(List<CraftCost> costs)
    {
        foreach (var cost in costs)
        {
            ResourceManager.Instance.Spend(cost.type.ToString(), cost.amount);
        }
    }

    public static string FormatCosts(List<CraftCost> costs)
    {
        if (costs == null) return "";
        var parts = new List<string>();
        foreach (var cost in costs)
        {
            parts.Add($"{cost.amount} {cost.type}");
        }
        return string.Join(", ", parts);
    }
}