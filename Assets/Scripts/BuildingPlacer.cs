using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    private enum PlaceableType { None, CraftingStation, Turret }

    [Header("Crafting Station")]
    public GameObject craftingStationPrefab;
    public int craftingStationCost = 3;
    public int maxCraftingStations = 1;

    [Header("Turret")]
    public GameObject turretPrefab;
    public List<CraftCost> turretCosts = new List<CraftCost>
    {
        new CraftCost { type = ResourceType.Copper, amount = 3 },
        new CraftCost { type = ResourceType.Iron, amount = 2 },
        new CraftCost { type = ResourceType.Gold, amount = 1 },
    };
    public LayerMask buildZoneLayerMask;

    [Header("Placement Settings")]
    public LayerMask obstacleLayerMask;
    public Color validColor = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private PlaceableType currentType = PlaceableType.None;
    private bool isPlacing;
    private GameObject ghostInstance;
    private SpriteRenderer ghostRenderer;
    private bool isValidPlacement;

    public List<CraftCost> GetTurretCosts() => turretCosts;

    private void Update()
    {
        // Turret placement is now triggered directly from the Hotbar when selected,
        // not from the B key. B is only for the Crafting Station.
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (!isPlacing) TryEnterPlacementMode(PlaceableType.CraftingStation);
            else CancelPlacement();
        }

        if (!isPlacing) return;

        UpdateGhostPosition();

        if (Mouse.current.leftButton.wasPressedThisFrame && isValidPlacement)
        {
            ConfirmPlacement();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    public void BeginPlacingTurret()
    {
        var inventory = GetComponent<PlayerInventory>();
        if (inventory == null || inventory.turretCount <= 0)
        {
            Debug.Log("No Turret available to place — craft one first");
            return;
        }

        TryEnterPlacementMode(PlaceableType.Turret);
    }

    private void TryEnterPlacementMode(PlaceableType type)
    {
        if (type == PlaceableType.CraftingStation)
        {
            if (CraftingStation.ActiveCount >= maxCraftingStations)
            {
                Debug.Log("Crafting Station limit reached");
                return;
            }
            if (ResourceManager.Instance.GetAmount("Copper") < craftingStationCost)
            {
                Debug.Log("Not enough Copper to place Crafting Station");
                return;
            }
        }
        // Turret's affordability/availability was already checked at craft time
        // and re-checked in BeginPlacingTurret() — no cost check needed here.

        currentType = type;
        isPlacing = true;

        GameObject prefab = type == PlaceableType.CraftingStation ? craftingStationPrefab : turretPrefab;

        ghostInstance = new GameObject("PlacementGhost");
        ghostRenderer = ghostInstance.AddComponent<SpriteRenderer>();

        var prefabRenderer = prefab.GetComponent<SpriteRenderer>();
        if (prefabRenderer != null)
        {
            ghostRenderer.sprite = prefabRenderer.sprite;
            ghostRenderer.sortingLayerID = prefabRenderer.sortingLayerID;
            ghostRenderer.sortingOrder = prefabRenderer.sortingOrder;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    private void UpdateGhostPosition()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        ghostInstance.transform.position = mouseWorldPos;

        bool obstacleFree = Physics2D.OverlapCircle(mouseWorldPos, 0.5f, obstacleLayerMask) == null;
        bool inZone = true;

        if (currentType == PlaceableType.Turret)
        {
            inZone = Physics2D.OverlapPoint(mouseWorldPos, buildZoneLayerMask) != null;
        }

        isValidPlacement = obstacleFree && inZone;

        if (ghostRenderer != null)
        {
            ghostRenderer.color = isValidPlacement ? validColor : invalidColor;
        }
    }

    private void ConfirmPlacement()
    {
        if (currentType == PlaceableType.CraftingStation)
        {
            ResourceManager.Instance.Spend("Copper", craftingStationCost);
            Instantiate(craftingStationPrefab, ghostInstance.transform.position, Quaternion.identity);
            Debug.Log($"Crafting Station placed! Spent {craftingStationCost} Copper.");
        }
        else if (currentType == PlaceableType.Turret)
        {
            var inventory = GetComponent<PlayerInventory>();
            inventory.SpendTurret(1);
            Instantiate(turretPrefab, ghostInstance.transform.position, Quaternion.identity);
            Debug.Log("Turret placed!");
        }

        Destroy(ghostInstance);
        isPlacing = false;
        currentType = PlaceableType.None;
    }

    private void CancelPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        isPlacing = false;
        currentType = PlaceableType.None;
    }
    public void CancelPlacementPublic()
    {
        CancelPlacement();
    }

}