using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [Header("NPC Identity")]
    public string npcName = "NPC";

    [Header("Dialogue Setup - By Day")]
    [SerializeField] private List<DialogueDirectoryByDay> dialoguesByDay = new List<DialogueDirectoryByDay>();

    [SerializeField] private List<string> dialogueSequence = new List<string>();

    [Header("Fallback Settings")]
    public bool useFallbackOnMissingDay = true;
    public int fallbackDay = 1;

    [Header("Dialogue UI")]
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource buttonSound;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private int currentDialogueIndex = 0;

    private GameObject player;

    private InteractableObject nearbyInteractable;


    void Start()
    {
        if (string.IsNullOrEmpty(npcName))
            Debug.LogError($"NPCDialogue on {gameObject.name} has no NPC name assigned!");

        if (dialoguesByDay.Count == 0)
            Debug.LogWarning($"NPCDialogue on {gameObject.name} has no dialogue directories assigned!");
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
        buttonSound.Play();
        // Block interaction if dialogue is already active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            if (showDebugMessages)
                Debug.Log($"Cannot interact with {gameObject.name} - dialogue already active!");
            return;
        }

        if (showDebugMessages)
            Debug.Log($"NPC {gameObject.name} interacted!");

        // Get the correct dialogue directory for the current day
        DialogueDirectory currentDirectory = GetDialogueDirectoryForCurrentDay();

        if (currentDirectory == null)
        {
            Debug.LogError($"No dialogue directory found for {gameObject.name} on Day {TimeSystem.Instance.currentDay}!");
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

        PlayCurrentDialogue(currentDirectory);
    }

    void PlayCurrentDialogue(DialogueDirectory currentDirectory)
    {
        if (currentDialogueIndex >= dialogueSequence.Count)
        {
            if (showDebugMessages)
                Debug.Log("All dialogues completed!");
            return;
        }

        string dialogueName = dialogueSequence[currentDialogueIndex];
        Dialogue dialogue = currentDirectory.GetDialogueByName(dialogueName);

        if (dialogue != null)
        {
            if (showDebugMessages)
                Debug.Log($"Playing dialogue: {dialogueName} (Day {TimeSystem.Instance.currentDay})");

            DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished, npcName);
        }
        else
        {
            Debug.LogError($"Dialogue '{dialogueName}' not found in directory for Day {TimeSystem.Instance.currentDay}!");
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

    // Get the correct dialogue directory for the current day
    DialogueDirectory GetDialogueDirectoryForCurrentDay()
    {
        int currentDay = TimeSystem.Instance.currentDay;

        // Look for exact day match
        foreach (DialogueDirectoryByDay dayDir in dialoguesByDay)
        {
            if (dayDir.day == currentDay && dayDir.directory != null)
            {
                if (showDebugMessages)
                    Debug.Log($"Using dialogue directory for Day {currentDay}");
                return dayDir.directory;
            }
        }

        // If no exact match and fallback is enabled
        if (useFallbackOnMissingDay)
        {
            foreach (DialogueDirectoryByDay dayDir in dialoguesByDay)
            {
                if (dayDir.day == fallbackDay && dayDir.directory != null)
                {
                    if (showDebugMessages)
                        Debug.Log($"No directory for Day {currentDay}. Using fallback Day {fallbackDay}");
                    return dayDir.directory;
                }
            }
        }

        // No directory found
        if (showDebugMessages)
            Debug.LogError($"No dialogue directory available for {gameObject.name} on Day {currentDay}");
        return null;
    }

    // Helper method to get current day's directory
    public DialogueDirectory GetCurrentDayDirectory()
    {
        return GetDialogueDirectoryForCurrentDay();
    }

    // Get all available days for this NPC
    public int[] GetAvailableDays()
    {
        List<int> days = new List<int>();
        foreach (DialogueDirectoryByDay dayDir in dialoguesByDay)
        {
            if (dayDir.directory != null && !days.Contains(dayDir.day))
            {
                days.Add(dayDir.day);
            }
        }
        return days.ToArray();
    }
}

