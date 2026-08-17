using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The child GameObject holding the world-space Canvas — shown/hidden as a whole.")]
    public GameObject barRoot;
    [Tooltip("Image with Image Type = Filled, Fill Method = Horizontal.")]
    public Image fillImage;

    [Header("Behavior")]
    public float lingerDuration = 10f;

    private Health health;
    private float timer;
    private bool isVisible;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (barRoot != null) barRoot.SetActive(false);
    }

    private void OnEnable()
    {
        if (health != null) health.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged()
    {
        UpdateFill();
        Show();
    }

    private void UpdateFill()
    {
        if (fillImage != null && health.maxHealth > 0)
        {
            fillImage.fillAmount = (float)health.CurrentHealth / health.maxHealth;
        }
    }

    private void Show()
    {
        if (barRoot != null) barRoot.SetActive(true);
        isVisible = true;
        timer = lingerDuration;
    }

    private void Update()
    {
        if (!isVisible) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Hide();
        }
    }

    private void Hide()
    {
        if (barRoot != null) barRoot.SetActive(false);
        isVisible = false;
    }
}