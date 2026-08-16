using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int c4Count = 0;
    public event Action OnInventoryChanged;

    public void AddC4(int amount)
    {
        c4Count += amount;
        OnInventoryChanged?.Invoke();
    }

    public bool SpendC4(int amount)
    {
        if (c4Count < amount) return false;
        c4Count -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }
}