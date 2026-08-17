using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance { get; private set; }

    [Header("Panel")]
    public GameObject panel;
    [Tooltip("Full-screen black Image, alpha 0 at rest.")]
    public Image fadeImage;
    [Tooltip("Holds the message + buttons, hidden until the fade completes.")]
    public CanvasGroup contentGroup;

    [Header("Content")]
    public TMP_Text messageText;
    public string completionMessage = "Level Complete!";
    public Button retryButton;
    public Button mainMenuButton;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float contentFadeDuration = 0.5f;

    [Header("Scenes")]
    [Tooltip("Exact scene name (as in Build Settings) to load for Main Menu.")]
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (panel != null) panel.SetActive(false);
        SetFadeAlpha(0f);

        if (contentGroup != null)
        {
            contentGroup.alpha = 0f;
            contentGroup.interactable = false;
            contentGroup.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        Time.timeScale = 0f;

        if (panel == null)
        {
            Debug.LogWarning("LevelCompleteUI: panel not assigned");
            return;
        }

        panel.SetActive(true);
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            SetFadeAlpha(Mathf.Clamp01(timer / fadeDuration));
            yield return null;
        }
        SetFadeAlpha(1f);

        if (contentGroup != null)
        {
            contentGroup.interactable = true;
            contentGroup.blocksRaycasts = true;

            float contentTimer = 0f;
            while (contentTimer < contentFadeDuration)
            {
                contentTimer += Time.unscaledDeltaTime;
                contentGroup.alpha = Mathf.Clamp01(contentTimer / contentFadeDuration);
                yield return null;
            }
            contentGroup.alpha = 1f;
        }
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}