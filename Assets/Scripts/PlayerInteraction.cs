using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private Collider2D currentInteractableCollider;

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (currentInteractable == null)
        {
            Debug.Log("No interactable nearby");
            return;
        }

        if (currentInteractable.CanInteract(gameObject))
        {
            currentInteractable.Interact(gameObject);
        }
        else
        {
            Debug.Log("CanInteract returned false — condition not met");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log($"Entered interact range: {other.gameObject.name}");
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