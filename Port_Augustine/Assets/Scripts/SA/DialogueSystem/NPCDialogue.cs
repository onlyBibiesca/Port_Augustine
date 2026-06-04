using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public DialogueDirectory dialogueDirectory;
    public List<string> dialogueSequence = new List<string>();

    [Header("Dialogue UI")]
    [SerializeField] GameObject interactUI;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private int currentDialogueIndex = 0;

    private GameObject player;

    private InteractableObject nearbyInteractable;

    void Start()
    {
        if (dialogueDirectory == null)
        {
            Debug.LogError($"No Dialogue Directory assigned to {gameObject.name}!");
        }

        if (dialogueSequence.Count == 0)
        {
            Debug.LogWarning($"No dialogue sequence assigned to {gameObject.name}!");
        }
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

    public void OnInteract()
    {
        // Block interaction if dialogue is already active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            if (showDebugMessages)
                Debug.Log($"Cannot interact with {gameObject.name} - dialogue already active!");
            return;
        }

        if (showDebugMessages)
            Debug.Log($"NPC {gameObject.name} interacted!");

        if (dialogueDirectory == null)
        {
            Debug.LogError("No dialogue directory assigned!");
            return;
        }

        if (dialogueSequence.Count == 0)
        {
            Debug.LogError("No dialogue sequence assigned!");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        PlayCurrentDialogue();
    }

    void PlayCurrentDialogue()
    {
        if (currentDialogueIndex >= dialogueSequence.Count)
        {
            if (showDebugMessages)
                Debug.Log("All dialogues completed!");
            return;
        }

        string dialogueName = dialogueSequence[currentDialogueIndex];
        Dialogue dialogue = dialogueDirectory.GetDialogueByName(dialogueName);

        if (dialogue != null)
        {
            if (showDebugMessages)
                Debug.Log($"Playing dialogue: {dialogueName}");

            DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
        }
        else
        {
            Debug.LogError($"Dialogue '{dialogueName}' not found in directory!");
        }
    }

    void OnDialogueFinished()
    {
        if (showDebugMessages)
            Debug.Log($"Dialogue finished. Moving to next dialogue. Current index: {currentDialogueIndex}");

        currentDialogueIndex++;
    }

    public void ResetDialogueSequence()
    {
        currentDialogueIndex = 0;
        Debug.Log("Dialogue sequence reset");
    }
}

