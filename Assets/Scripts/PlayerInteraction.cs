using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private Collider2D currentInteractableCollider;

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (currentInteractable == null) return;

        if (currentInteractable.CanInteract(gameObject))
        {
            currentInteractable.Interact(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractableCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentInteractableCollider)
        {
            currentInteractable = null;
            currentInteractableCollider = null;
        }
    }
}