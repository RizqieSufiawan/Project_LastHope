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

        Debug.Log("Advancing to next level!");
    }

    public string GetPrompt()
    {
        return CanInteract(null) ? "Press E to advance" : "Blockade still active";
    }
}