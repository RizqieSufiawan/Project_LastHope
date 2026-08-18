using UnityEngine;
using UnityEngine.UI;

public class MiningProgressBarUI : MonoBehaviour
{
    public static MiningProgressBarUI Instance { get; private set; }

    [Header("Referensi UI")]
    [Tooltip("GameObject induk dari Progress Bar (biasanya berisi Canvas Group atau background).")]
    public GameObject barRoot;

    [Tooltip("Image dengan Image Type = Filled, Fill Method = Horizontal.")]
    public Image fillImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Hide();
    }

    public void UpdateProgress(float currentHit, float requiredHits)
    {
        if (barRoot != null && !barRoot.activeSelf)
        {
            barRoot.SetActive(true);
        }

        if (fillImage != null && requiredHits > 0)
        {
            fillImage.fillAmount = currentHit / requiredHits;
        }
    }

    public void Hide()
    {
        if (barRoot != null) barRoot.SetActive(false);
    }
}