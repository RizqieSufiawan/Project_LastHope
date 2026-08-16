using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public static HotbarUI Instance { get; private set; }
    [System.Serializable]
    public class HotbarSlot
    {
        public Image background;
        public Image icon;
        public TMP_Text countText;
    }

    public GameObject player;
    public HotbarSlot[] slots = new HotbarSlot[4];

    [Header("Pickaxe Icons")]
    public Sprite pickaxeBaseIcon;
    public Sprite pickaxeIronIcon;
    public Sprite pickaxeGoldIcon;
    public Sprite pickaxeDiamondIcon;

    [Header("Item Icons")]
    public Sprite c4Icon;
    public Sprite grenadeIcon;
    public Sprite turretIcon;

    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private PlayerMining mining;
    private PlayerInventory inventory;
    private BuildingPlacer placer;
    private int selectedIndex = -1;

    private void Awake()
    {
        Instance = this;
        mining = player.GetComponent<PlayerMining>();
        inventory = player.GetComponent<PlayerInventory>();
        placer = player.GetComponent<BuildingPlacer>();
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlot(3);
        }

        RefreshSlots();
        RefreshHighlight();
    }

    public void OnClickSlot(int index)
    {
        SelectSlot(index);
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        if (index == 0)
        {
            if (selectedIndex == 0 && mining != null)
            {
                mining.ToggleEquip();
                selectedIndex = -1;
                return;
            }
            else if (mining != null && !mining.isEquipped)
            {
                mining.ToggleEquip();
            }
        }
        else
        {
            if (selectedIndex == index)
            {
                selectedIndex = -1;
                return;
            }

            if (mining != null && mining.isEquipped)
            {
                mining.ToggleEquip();
            }
        }

        selectedIndex = index;

        string itemId = GetSelectedItemIdInternal(index);
        if (itemId == "Turret" && inventory != null && inventory.GetItemCount("Turret") > 0 && placer != null)
        {
            placer.BeginPlacingTurret();
        }
    }

    private string GetSelectedItemIdInternal(int index)
    {
        if (index <= 0) return null;
        int orderIndex = index - 1;
        if (orderIndex >= 0 && orderIndex < inventory.acquisitionOrder.Count)
        {
            return inventory.acquisitionOrder[orderIndex];
        }
        return null;
    }

    private void RefreshHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i]?.background == null) continue;
            slots[i].background.color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }

    private void RefreshSlots()
    {
        if (slots.Length > 0 && slots[0] != null && mining != null)
        {
            slots[0].icon.enabled = true;
            slots[0].icon.sprite = GetPickaxeIcon(mining.currentPickaxeLevel);
            if (slots[0].countText != null) slots[0].countText.text = "";
        }

        for (int i = 1; i < slots.Length; i++)
        {
            int orderIndex = i - 1;

            if (orderIndex < inventory.acquisitionOrder.Count)
            {
                string itemId = inventory.acquisitionOrder[orderIndex];
                int count = inventory.GetItemCount(itemId);

                if (count > 0)
                {
                    slots[i].icon.enabled = true;
                    slots[i].icon.sprite = GetItemIcon(itemId);
                    if (slots[i].countText != null) slots[i].countText.text = count.ToString();
                }
                else
                {
                    slots[i].icon.enabled = false;
                    if (slots[i].countText != null) slots[i].countText.text = "";
                }
            }
            else
            {
                slots[i].icon.enabled = false;
                if (slots[i].countText != null) slots[i].countText.text = "";
            }
        }
    }

    private Sprite GetItemIcon(string itemId)
    {
        switch (itemId)
        {
            case "C4": return c4Icon;
            case "Grenade": return grenadeIcon;
            case "Turret": return turretIcon;
            default: return null;
        }
    }

    private Sprite GetPickaxeIcon(PickaxeLevel level)
    {
        switch (level)
        {
            case PickaxeLevel.Base: return pickaxeBaseIcon;
            case PickaxeLevel.Iron: return pickaxeIronIcon;
            case PickaxeLevel.Gold: return pickaxeGoldIcon;
            case PickaxeLevel.Diamond: return pickaxeDiamondIcon;
            default: return pickaxeBaseIcon;
        }
    }

    public string GetSelectedItemId()
    {
        return GetSelectedItemIdInternal(selectedIndex);
    }
}