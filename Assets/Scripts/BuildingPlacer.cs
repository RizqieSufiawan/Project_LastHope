using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Crafting Station")]
    public GameObject craftingStationPrefab;
    public int craftingStationCost = 3;
    public int maxCraftingStations = 1;

    [Header("Placement Settings")]
    public LayerMask obstacleLayerMask;
    public Color validColor = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private bool isPlacing;
    private GameObject ghostInstance;
    private SpriteRenderer ghostRenderer;
    private bool isValidPlacement;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (!isPlacing)
            {
                TryEnterPlacementMode();
            }
            else
            {
                CancelPlacement();
            }
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

    private void TryEnterPlacementMode()
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

        isPlacing = true;

        ghostInstance = new GameObject("PlacementGhost");
        ghostRenderer = ghostInstance.AddComponent<SpriteRenderer>();

        var prefabRenderer = craftingStationPrefab.GetComponent<SpriteRenderer>();
        if (prefabRenderer != null)
        {
            ghostRenderer.sprite = prefabRenderer.sprite;
            ghostRenderer.sortingLayerID = prefabRenderer.sortingLayerID;
            ghostRenderer.sortingOrder = prefabRenderer.sortingOrder;
        }
    }
    private void UpdateGhostPosition()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        ghostInstance.transform.position = mouseWorldPos;

        Collider2D overlap = Physics2D.OverlapCircle(mouseWorldPos, 0.5f, obstacleLayerMask);
        isValidPlacement = overlap == null;

        if (ghostRenderer != null)
        {
            ghostRenderer.color = isValidPlacement ? validColor : invalidColor;
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

    private void ConfirmPlacement()
    {
        ResourceManager.Instance.Spend("Copper", craftingStationCost);

        Instantiate(craftingStationPrefab, ghostInstance.transform.position, Quaternion.identity);
        Debug.Log($"Crafting Station placed! Spent {craftingStationCost} Copper.");

        Destroy(ghostInstance);
        isPlacing = false;
    }

    private void CancelPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        isPlacing = false;
    }
}