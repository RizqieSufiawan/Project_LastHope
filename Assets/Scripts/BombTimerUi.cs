using UnityEngine;
using TMPro;

public class BombTimerUI : MonoBehaviour
{
    public static BombTimerUI Instance { get; private set; }

    [Tooltip("Parent GameObject yang di-show/hide sebagai satu kesatuan.")]
    public GameObject barRoot;
    public TMP_Text timerText;

    [Header("Ticking Audio")]
    public AudioClip tickClip;
    [Tooltip("Interval antar tick normal (detik).")]
    public float normalTickInterval = 1f;
    [Tooltip("Interval antar tick pas waktu tersisa di bawah threshold (detik) — lebih cepat/mendesak.")]
    public float fastTickInterval = 0.4f;
    [Tooltip("Di bawah berapa detik sisa waktu, tick mulai jadi cepat.")]
    public float fastTickThreshold = 10f;

    private float remainingTime;
    private bool isCounting;
    private float tickTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Hide();
    }

    public void StartCountdown(float duration)
    {
        remainingTime = duration;
        isCounting = true;
        tickTimer = 0f;
        if (barRoot != null) barRoot.SetActive(true);
        UpdateText();
    }

    private void Update()
    {
        if (!isCounting) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isCounting = false;
        }
        UpdateText();
        HandleTicking();
    }

    private void HandleTicking()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            AudioManager.Instance?.PlaySFX(tickClip);

            float interval = remainingTime <= fastTickThreshold ? fastTickInterval : normalTickInterval;
            tickTimer = interval;
        }
    }

    private void UpdateText()
    {
        if (timerText == null) return;
        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"{minutes}:{seconds:00}";
    }

    public void Hide()
    {
        isCounting = false;
        if (barRoot != null) barRoot.SetActive(false);
    }
}