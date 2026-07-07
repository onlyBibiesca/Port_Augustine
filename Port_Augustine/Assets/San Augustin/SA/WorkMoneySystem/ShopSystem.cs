using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopSystem : MonoBehaviour, InteractableObject
{
    // Start is called before the first frame update
    private GameObject player;

    private PlayerInput playerInput;
    private bool playerMovementEnabled = true;

    private InteractableObject nearbyInteractable;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource buttonSound;

    [Header("Shop")]
    [SerializeField] GameObject shopUI;

    

    public void Interact()
    {
        shopUI.SetActive(true);
        buttonSound.Play();
        DisablePlayerMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            InteractableObject interactable = collision.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                nearbyInteractable = interactable;
                if (interactUI != null)
                    interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractableObject>() == nearbyInteractable)
        {
            nearbyInteractable = null;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
    void FindPlayerInput()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogWarning("PlayerInput component not found!");
            }
        }
    }

    public void DisablePlayerMovement()
    {
        FindPlayerInput();
        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerMovementEnabled = false;
            Debug.Log("Player movement disabled");
        }
    }

    public void EnablePlayerMovement()
    {
        FindPlayerInput();
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerMovementEnabled = true;
            Debug.Log("Player movement enabled");
        }
    }
}
