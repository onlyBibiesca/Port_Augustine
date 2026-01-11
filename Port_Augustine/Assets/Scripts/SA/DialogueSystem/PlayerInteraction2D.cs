using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private NPC_Dialogue currentNPC;
    private bool isEnabled = true;

    void Update()
    {
        // Completely disable functionality if dialogue is active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            if (isEnabled)
            {
                isEnabled = false;
                if (showDebugMessages)
                    Debug.Log("Player interaction disabled - dialogue active");
            }
            return;
        }
        else if (!isEnabled)
        {
            isEnabled = true;
            if (showDebugMessages)
                Debug.Log("Player interaction enabled - dialogue ended");
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (showDebugMessages)
                Debug.Log("Interact key pressed!");

            if (currentNPC != null)
            {
                if (showDebugMessages)
                    Debug.Log($"Interacting with {currentNPC.gameObject.name}");

                currentNPC.OnInteract();
            }
            else
            {
                if (showDebugMessages)
                    Debug.Log("No NPC nearby");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Don't register new NPCs if dialogue is active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        NPC_Dialogue npc = other.GetComponent<NPC_Dialogue>();
        if (npc != null)
        {
            currentNPC = npc;
            if (showDebugMessages)
                Debug.Log($"Entered NPC trigger: {other.gameObject.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        NPC_Dialogue npc = other.GetComponent<NPC_Dialogue>();
        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
            if (showDebugMessages)
                Debug.Log($"Exited NPC trigger: {other.gameObject.name}");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Clear current NPC reference when dialogue becomes active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive && currentNPC != null)
        {
            currentNPC = null;
        }
    }
}
