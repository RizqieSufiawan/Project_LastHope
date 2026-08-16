using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class CraftingMenuUI : MonoBehaviour
{
    public static CraftingMenuUI Instance { get; private set; }

    [Header("Panel")]
    public GameObject panel;

    [Header("C4 Button")]
    public Button c4Button;
    public TMP_Text c4Label;

    [Header("Pickaxe Upgrade Button")]
    public Button upgradeButton;
    public TMP_Text upgradeLabel;
    public Image upgradeResultIcon;

    [Header("Pickaxe Tier Icons")]
    public Sprite baseIcon;
    public Sprite ironIcon;
    public Sprite goldIcon;
    public Sprite diamondIcon;


    [Header("Grenade Button (locked)")]
    public Button grenadeButton;
    public TMP_Text grenadeLabel;

    [Header("Turret Button (locked)")]
    public Button turretButton;
    public TMP_Text turretLabel;

    [System.Serializable]
    public class ResourceIconEntry
    {
        public ResourceType type;
        public Sprite icon;
    }

    [Header("Resource Icons")]
    public List<ResourceIconEntry> resourceIcons;

    [Header("C4 Slots")]
    public CraftCostSlotUI[] c4Slots = new CraftCostSlotUI[4];

    [Header("Upgrade Slots")]
    public CraftCostSlotUI[] upgradeSlots = new CraftCostSlotUI[4];

    [Header("Grenade Slots")]
    public CraftCostSlotUI[] grenadeSlots = new CraftCostSlotUI[4];

    [Header("Turret Slots")]
    public CraftCostSlotUI[] turretSlots = new CraftCostSlotUI[4];

    private CraftingStation currentStation;
    private GameObject currentPlayer;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (panel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Open(CraftingStation station, GameObject player)
    {
        currentStation = station;
        currentPlayer = player;
        panel.SetActive(true);
        RefreshButtons();

        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        var mining = player.GetComponent<PlayerMining>();
        if (mining != null) mining.enabled = false;
    }

    public void Close()
    {
        panel.SetActive(false);

        if (currentPlayer != null)
        {
            var movement = currentPlayer.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = true;
            var mining = currentPlayer.GetComponent<PlayerMining>();
            if (mining != null) mining.enabled = true;
        }

        currentStation = null;
        currentPlayer = null;
    }

    private void RefreshButtons()
    {
        if (currentStation == null) { Debug.LogError("currentStation is NULL"); return; }
        if (currentPlayer == null) { Debug.LogError("currentPlayer is NULL"); return; }
        if (ResourceManager.Instance == null) { Debug.LogError("ResourceManager.Instance is NULL"); return; }
        if (c4Button == null) { Debug.LogError("c4Button is NULL"); return; }
        if (upgradeButton == null) { Debug.LogError("upgradeButton is NULL"); return; }
        if (grenadeButton == null) { Debug.LogError("grenadeButton is NULL"); return; }
        if (turretButton == null) { Debug.LogError("turretButton is NULL"); return; }

        var mining = currentPlayer.GetComponent<PlayerMining>();
        if (mining == null) { Debug.LogError("PlayerMining component is NULL on currentPlayer"); return; }

        var c4Costs = currentStation.GetC4Costs();
        PopulateCostSlots(c4Slots, c4Costs);
        var inv = currentPlayer.GetComponent<PlayerInventory>();
        c4Button.interactable = CraftCostUtility.CanAfford(c4Costs) && inv.CanCraftC4();

        var upgradeCosts = currentStation.GetPickaxeUpgradeCosts(mining.currentPickaxeLevel);
        if (upgradeCosts == null)
        {
            PopulateCostSlots(upgradeSlots, null);
            upgradeButton.interactable = false;
            if (upgradeLabel != null) upgradeLabel.text = "Max Level";
        }
        else
        {
            PopulateCostSlots(upgradeSlots, upgradeCosts);
            upgradeButton.interactable = CraftCostUtility.CanAfford(upgradeCosts);

            PickaxeLevel nextLevel = mining.currentPickaxeLevel switch
            {
                PickaxeLevel.Base => PickaxeLevel.Iron,
                PickaxeLevel.Iron => PickaxeLevel.Gold,
                PickaxeLevel.Gold => PickaxeLevel.Diamond,
                _ => mining.currentPickaxeLevel
            };

            if (upgradeResultIcon != null) upgradeResultIcon.sprite = GetPickaxeTierIcon(nextLevel);
            if (upgradeLabel != null) upgradeLabel.text = $"Upgrade to {nextLevel}";
        }

        PopulateCostSlots(grenadeSlots, null);
        grenadeButton.interactable = false;

        var placer = currentPlayer.GetComponent<BuildingPlacer>();
        var turretCosts = placer != null ? placer.GetTurretCosts() : null;
        PopulateCostSlots(turretSlots, turretCosts);
        var invForTurret = currentPlayer.GetComponent<PlayerInventory>();
        turretButton.interactable = turretCosts != null && CraftCostUtility.CanAfford(turretCosts) && invForTurret.CanCraftTurret();
    }

    public void OnClickCraftTurret()
    {
        var placer = currentPlayer.GetComponent<BuildingPlacer>();
        var inventory = currentPlayer.GetComponent<PlayerInventory>();
        if (placer == null || inventory == null) return;

        var costs = placer.GetTurretCosts();
        if (!CraftCostUtility.CanAfford(costs)) return;
        if (!inventory.CanCraftTurret()) return;

        CraftCostUtility.Spend(costs);
        inventory.AddTurret(1);
        Debug.Log("Crafted 1 Turret! Select it in the Hotbar to place it.");

        RefreshButtons();
    }

    private Sprite GetPickaxeTierIcon(PickaxeLevel level)
    {
        return level switch
        {
            PickaxeLevel.Base => baseIcon,
            PickaxeLevel.Iron => ironIcon,
            PickaxeLevel.Gold => goldIcon,
            PickaxeLevel.Diamond => diamondIcon,
            _ => baseIcon
        };
    }

    private static readonly ResourceType[] SlotColumnOrder =
{
    ResourceType.Copper, ResourceType.Iron, ResourceType.Gold, ResourceType.Diamond
};

    private void PopulateCostSlots(CraftCostSlotUI[] slots, List<CraftCost> costs)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            ResourceType columnType = SlotColumnOrder[i];
            CraftCost match = costs?.Find(c => c.type == columnType);

            if (match != null)
            {
                Sprite icon = GetResourceIcon(match.type);
                slots[i].SetCost(icon, match.amount);
            }
            else
            {
                Sprite icon = GetResourceIcon(columnType);
                slots[i].SetCost(icon, 0); // correct icon for that column, just 0 needed
            }
        }
    }

    private Sprite GetResourceIcon(ResourceType type)
    {
        foreach (var entry in resourceIcons)
        {
            if (entry.type == type) return entry.icon;
        }
        return null;
    }

    public void OnClickCraftC4()
    {
        var costs = currentStation.GetC4Costs();
        var inventory = currentPlayer.GetComponent<PlayerInventory>();

        if (!CraftCostUtility.CanAfford(costs)) return;
        if (!inventory.CanCraftC4()) return;

        CraftCostUtility.Spend(costs);
        inventory.AddC4(1);
        Debug.Log("Crafted 1 C4!");

        RefreshButtons();
    }

    public void OnClickUpgradePickaxe()
    {
        var mining = currentPlayer.GetComponent<PlayerMining>();
        var costs = currentStation.GetPickaxeUpgradeCosts(mining.currentPickaxeLevel);
        if (costs == null || !CraftCostUtility.CanAfford(costs)) return;

        CraftCostUtility.Spend(costs);

        PickaxeLevel nextLevel = mining.currentPickaxeLevel switch
        {
            PickaxeLevel.Base => PickaxeLevel.Iron,
            PickaxeLevel.Iron => PickaxeLevel.Gold,
            PickaxeLevel.Gold => PickaxeLevel.Diamond,
            _ => mining.currentPickaxeLevel
        };

        mining.UpgradePickaxe(nextLevel);
        Debug.Log($"Pickaxe upgraded to {nextLevel}!");

        RefreshButtons();
    }
}