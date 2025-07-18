using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractableUI : MonoBehaviour, InteractableObject
{
    private InteractableObject interactableObject;
    public GameObject interactUI;

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }
    public void Interact()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InteractableObject interactable = collision.GetComponent<InteractableObject>();

        if (interactable != null)
        {
            interactableObject = interactable;
            if (interactUI != null)
                interactUI.SetActive(true);
        }
        /*if (collision.gameObject.CompareTag("Player"))
        {
            interactUI.SetActive(true);
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractableObject>() == interactableObject)
        {
            interactableObject = null;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
        /*if (collision.gameObject.CompareTag("Player"))
        {
            interactUI.SetActive(false);
        }*/
    }

}
