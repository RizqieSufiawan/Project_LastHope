using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int c4Count = 0;
    public int maxC4 = 1;

    public int turretCount = 0;

    public List<string> acquisitionOrder = new List<string>();

    public event Action OnInventoryChanged;

    public bool CanCraftC4()
    {
        return c4Count < maxC4;
    }

    public bool AddC4(int amount)
    {
        if (c4Count >= maxC4) return false;

        if (!acquisitionOrder.Contains("C4"))
        {
            acquisitionOrder.Add("C4");
            Debug.Log("C4 added to acquisitionOrder. Current list: " + string.Join(", ", acquisitionOrder));
        }

        c4Count = Mathf.Min(c4Count + amount, maxC4);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool SpendC4(int amount)
    {
        if (c4Count < amount) return false;
        c4Count -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool CanCraftTurret()
    {
        return true;
    }

    public bool AddTurret(int amount)
    {
        if (!acquisitionOrder.Contains("Turret"))
        {
            acquisitionOrder.Add("Turret");
            Debug.Log("Turret added to acquisitionOrder. Current list: " + string.Join(", ", acquisitionOrder));
        }

        turretCount += amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool SpendTurret(int amount)
    {
        if (turretCount < amount) return false;
        turretCount -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetItemCount(string itemId)
    {
        switch (itemId)
        {
            case "C4": return c4Count;
            case "Turret": return turretCount;
            default: return 0;
        }
    }
}