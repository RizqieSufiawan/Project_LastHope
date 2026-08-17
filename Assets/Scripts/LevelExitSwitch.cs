using UnityEngine;

public class LevelExitSwitch : MonoBehaviour, IInteractable
{
    public GateController linkedGate;

    public bool CanInteract(GameObject player)
    {
        return linkedGate != null && linkedGate.IsDestroyed;
    }

    public void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;

        if (ConfirmationDialogUI.Instance != null)
        {
            ConfirmationDialogUI.Instance.Open(
                "Advance to the next level?",
                HandleAdvanceConfirmed,
                null,
                player
            );
        }
        else
        {
            Debug.LogWarning("ConfirmationDialogUI.Instance is NULL — advancing without confirmation.");
            HandleAdvanceConfirmed();
        }
    }

    private void HandleAdvanceConfirmed()
    {
        if (LevelCompleteUI.Instance != null)
        {
            LevelCompleteUI.Instance.Show();
        }
        else
        {
            Debug.LogWarning("LevelCompleteUI.Instance is NULL — no completion screen shown.");
        }
    }

    public string GetPrompt()
    {
        return CanInteract(null) ? "Press E to advance" : "Blockade still active";
    }
}