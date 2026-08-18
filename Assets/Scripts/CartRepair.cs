using UnityEngine;

[RequireComponent(typeof(Health))]
public class CartRepair : MonoBehaviour, IInteractable
{
    public int copperPerHeal = 1; 
    public int hpPerCopper = 5;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public bool CanInteract(GameObject player)
    {
        if (health.CurrentHealth >= health.maxHealth) return false;

        int missingHp = health.maxHealth - health.CurrentHealth;
        int copperNeeded = Mathf.CeilToInt((float)missingHp / hpPerCopper) * copperPerHeal;
        int copperNeededClamped = Mathf.Max(copperPerHeal, copperNeeded);

        return ResourceManager.Instance.GetAmount("Copper") >= copperPerHeal;
    }

    public void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;

        int missingHp = health.maxHealth - health.CurrentHealth;
        int availableCopper = ResourceManager.Instance.GetAmount("Copper");

        int maxHealableByCopper = (availableCopper / copperPerHeal) * hpPerCopper;
        int healAmount = Mathf.Min(missingHp, maxHealableByCopper);
        int copperToSpend = Mathf.CeilToInt((float)healAmount / hpPerCopper) * copperPerHeal;

        ResourceManager.Instance.Spend("Copper", copperToSpend);
        health.Heal(healAmount);

    }

    public string GetPrompt()
    {
        if (health.CurrentHealth >= health.maxHealth) return "Cart at full health";
        return "Press E to repair Cart (Copper)";
    }
}