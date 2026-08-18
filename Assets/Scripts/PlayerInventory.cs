using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int c4Count = 0;
    public int maxC4 = 1;

    public int turretCount = 0;

    public int grenadeCount = 0;

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

    public bool CanCraftGrenade()
    {
        return true;
    }

    public bool AddGrenade(int amount)
    {
        if (!acquisitionOrder.Contains("Grenade"))
        {
            acquisitionOrder.Add("Grenade");
        }

        grenadeCount += amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool SpendGrenade(int amount)
    {
        if (grenadeCount < amount) return false;
        grenadeCount -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetItemCount(string itemId)
    {
        switch (itemId)
        {
            case "C4": return c4Count;
            case "Turret": return turretCount;
            case "Grenade": return grenadeCount;
            default: return 0;
        }
    }
}