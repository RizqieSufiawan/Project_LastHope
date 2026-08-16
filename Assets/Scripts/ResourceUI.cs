using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSlot
    {
        public ResourceType type;
        public Image icon;
        public TMP_Text amountText;
    }

    public ResourceSlot[] slots;

    private bool isSubscribed;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (isSubscribed && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
        }
        isSubscribed = false;
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribe();
        }
    }

    private void TrySubscribe()
    {
        if (isSubscribed) return;
        if (ResourceManager.Instance == null) return;

        ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
        isSubscribed = true;
        RefreshAll();
    }

    private void HandleResourceChanged(string resourceType, int newAmount)
    {
        foreach (var slot in slots)
        {
            if (slot.type.ToString() == resourceType)
            {
                if (slot.amountText != null) slot.amountText.text = newAmount.ToString();
                break;
            }
        }
    }

    private void RefreshAll()
    {
        if (ResourceManager.Instance == null) return;

        foreach (var slot in slots)
        {
            int amount = ResourceManager.Instance.GetAmount(slot.type.ToString());
            if (slot.amountText != null) slot.amountText.text = amount.ToString();
        }
    }
}