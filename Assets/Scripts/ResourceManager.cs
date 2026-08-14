using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public static ResourceManager Instance { get; private set; }

    private Dictionary<string, int> totals = new Dictionary<string, int>();

    public event Action<string, int> OnResourceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int GetAmount(string resourceType)
    {
        return totals.TryGetValue(resourceType, out int amount) ? amount : 0;
    }

    public void Add(string resourceType, int amount)
    {
        if (!totals.ContainsKey(resourceType)) totals[resourceType] = 0;
        totals[resourceType] += amount;
        OnResourceChanged?.Invoke(resourceType, totals[resourceType]);
    }

    public bool Spend(string resourceType, int amount)
    {
        if (GetAmount(resourceType) < amount) return false;

        totals[resourceType] -= amount;
        OnResourceChanged?.Invoke(resourceType, totals[resourceType]);
        return true;
    }
}
