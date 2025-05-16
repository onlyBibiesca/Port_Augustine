using UnityEngine;
using UnityEngine.InputSystem;

public class Interactions : MonoBehaviour
{
    private InteractableObject nearbyInteractable;
    [SerializeField] private GameObject promptUI; // Assign UI prompt in the Inspector

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InteractableObject interactable = collision.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            nearbyInteractable = interactable;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractableObject>() == nearbyInteractable)
        {
            nearbyInteractable = null;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyInteractable != null)
        {
            nearbyInteractable.Interact();
        }
    }
}