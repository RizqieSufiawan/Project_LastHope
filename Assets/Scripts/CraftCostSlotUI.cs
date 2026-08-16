using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftCostSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void SetCost(Sprite sprite, int amount)
    {
        gameObject.SetActive(true);
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }
        if (amountText != null) amountText.text = amount.ToString();
    }

    public void Clear()
    {
        gameObject.SetActive(true);
        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.text = "0";
    }
}