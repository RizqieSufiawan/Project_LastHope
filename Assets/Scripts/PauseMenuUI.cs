using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance { get; private set; }

    [Header("Panel")]
    public GameObject panel;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (panel != null) panel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (IsOtherBlockingUIOpen()) return;

        if (panel != null && panel.activeSelf) Resume();
        else Pause();
    }

    private bool IsOtherBlockingUIOpen()
    {
        if (CraftingMenuUI.Instance != null && CraftingMenuUI.Instance.panel.activeSelf) return true;
        if (ConfirmationDialogUI.Instance != null && ConfirmationDialogUI.Instance.panel.activeSelf) return true;
        if (LevelCompleteUI.Instance != null && LevelCompleteUI.Instance.panel.activeSelf) return true;
        if (FailMenuUI.Instance != null && FailMenuUI.Instance.panel.activeSelf) return true;
        return false;
    }

    public void Pause()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickResume()
    {
        Resume();
    }

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}