using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationDialogUI : MonoBehaviour
{
    public static ConfirmationDialogUI Instance { get; private set; }

    [Header("Panel")]
    public GameObject panel;
    public TMP_Text messageText;
    public Button yesButton;
    public Button noButton;

    private Action onConfirm;
    private Action onCancel;
    private GameObject currentPlayer;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(string message, Action confirmCallback, Action cancelCallback = null, GameObject player = null)
    {
        onConfirm = confirmCallback;
        onCancel = cancelCallback;
        currentPlayer = player;

        if (messageText != null) messageText.text = message;
        panel.SetActive(true);

        if (currentPlayer != null)
        {
            var movement = currentPlayer.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = false;
            var mining = currentPlayer.GetComponent<PlayerMining>();
            if (mining != null) mining.enabled = false;
        }
    }

    public void OnClickYes()
    {
        Close();
        if (LevelCompleteUI.Instance != null)
        {
            LevelCompleteUI.Instance.Show();
        }
    }

    public void OnClickNo()
    {
        Close();
        onCancel?.Invoke();
    }

    private void Close()
    {
        panel.SetActive(false);

        if (currentPlayer != null)
        {
            var movement = currentPlayer.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = true;
            var mining = currentPlayer.GetComponent<PlayerMining>();
            if (mining != null) mining.enabled = true;
        }

        onConfirm = null;
        onCancel = null;
        currentPlayer = null;
    }
}