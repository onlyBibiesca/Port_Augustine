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

    [Header("Event System")]
    public List<NPCEvent> availableEvents = new List<NPCEvent>();

    private NPCEvent currentEvent = null;
    private string currentEventKey = "";
    private Vector3 originalPosition;

    [Header("Dialogue UI")]
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource buttonSound;

    [Header("Debug")]
    public bool showDebugMessages = true;
    [SerializeField] int minimumEnergy;

    private PlayerStats playerStats;

    private int currentDialogueIndex = 0;
    private int firstInteractionDay = -1; // Track first day NPC was interacted with (real game day)
    private int lastInteractionDay = -1; // Track the last day we interacted
    private int currentProgressionDay = 1; // Which day's dialogue set we're on (1, 2, 3, etc.)

    private GameObject player;

    private InteractableObject nearbyInteractable;


    void Start()
    {
        originalPosition = transform.position;
        CheckForAvailableEvents();

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

    void CheckForAvailableEvents()
    {
        if (RelationshipManager.Instance == null || NPCEventManager.Instance == null)
            return;

        int currentRelationship = RelationshipManager.Instance.GetRelationship(npcName);
        NPCEvent availableEvent = NPCEventManager.Instance.GetAvailableEvent(npcName, currentRelationship, availableEvents);

        if (availableEvent != null)
        {
            currentEvent = availableEvent;
            currentEventKey = $"{npcName}_{availableEvent.eventName}";

            // Move NPC to event location
            if (availableEvent.eventLocation != null)
            {
                transform.position = availableEvent.eventLocation.position;
            }

            // Show exclamation mark
            NPCEventIndicator indicator = GetComponent<NPCEventIndicator>();
            if (indicator != null)
                indicator.ShowEventIndicator();

            Debug.Log($"Event available: {currentEvent.eventName}");
        }
    }


    public void OnInteract()
    {
        if(PlayerStats.Instance.energy >= minimumEnergy)
        {
            buttonSound.Play();
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                if (showDebugMessages)
                    Debug.Log($"Cannot interact with {gameObject.name} - dialogue already active!");
                return;
            }

            int currentGameDay = TimeSystem.Instance.currentDay;

            // Track first interaction day (real game day)
            if (firstInteractionDay == -1)
            {
                firstInteractionDay = currentGameDay;
                lastInteractionDay = currentGameDay;
                currentProgressionDay = fallbackDay; // Start with fallback day (Day 1)
                if (showDebugMessages)
                    Debug.Log($"{npcName} first interacted on Day {firstInteractionDay}. Starting with progression day {currentProgressionDay}");
            }
            else
            {
                // Check if a new day has passed since last interaction
                if (currentGameDay > lastInteractionDay)
                {
                    currentProgressionDay++;
                    if (showDebugMessages)
                        Debug.Log($" New day detected! Progression day advanced to {currentProgressionDay}");
                }
                lastInteractionDay = currentGameDay;
            }

            if (showDebugMessages)
                Debug.Log($"NPC {gameObject.name} interacted! Using progression day {currentProgressionDay}");

            // Get the correct dialogue directory based on progression
            DialogueDirectory currentDirectory = GetDialogueDirectoryForDay(currentProgressionDay);

            if (currentDirectory == null)
            {
                Debug.LogError($"No dialogue directory found for {gameObject.name}!");
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
        else if(PlayerStats.Instance.energy < minimumEnergy)
        {
            Debug.Log("Not enough energy");
        }
        
    }

    void OnEventFinished()
    {
        bool eventSuccess = NPCEventManager.Instance.IsEventSuccessful(currentEventKey);

        // Apply outcome
        if (eventSuccess)
        {
            Debug.Log("Event successful!");
            if (currentEvent.successDialogue != null)
                DialogueManager.Instance.StartDialogue(currentEvent.successDialogue, null, npcName);

            RelationshipManager.Instance.ChangeRelationship(npcName, currentEvent.successRelationshipBonus);
        }
        else
        {
            Debug.Log("Event failed!");
            if (currentEvent.failureDialogue != null)
                DialogueManager.Instance.StartDialogue(currentEvent.failureDialogue, null, npcName);

            RelationshipManager.Instance.ChangeRelationship(npcName, currentEvent.failureRelationshipPenalty);
        }

        NPCEventManager.Instance.CompleteEvent(currentEventKey, eventSuccess);
        currentEvent = null;

        // Return NPC to original position
        transform.position = originalPosition;

        // Hide indicator
        NPCEventIndicator indicator = GetComponent<NPCEventIndicator>();
        if (indicator != null)
            indicator.HideEventIndicator();
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
    DialogueDirectory GetDialogueDirectoryForProgression()
    {
        // ALWAYS start with fallback day on first interaction
        if (firstInteractionDay == -1)
        {
            if (showDebugMessages)
                Debug.Log($"First interaction detected. Using fallback Day {fallbackDay}");
            return GetDialogueDirectoryForDay(fallbackDay);
        }

        // After first interaction, use the tracked progression day
        DialogueDirectory directory = GetDialogueDirectoryForDay(firstInteractionDay);

        if (directory != null)
        {
            if (showDebugMessages)
                Debug.Log($"Using dialogue directory from Day {firstInteractionDay}");
            return directory;
        }

        // If tracked day directory doesn't exist, use fallback
        if (useFallbackOnMissingDay)
        {
            directory = GetDialogueDirectoryForDay(fallbackDay);
            if (directory != null)
            {
                if (showDebugMessages)
                    Debug.Log($"Day {firstInteractionDay} directory not found. Using fallback Day {fallbackDay}");
                return directory;
            }
        }

        return null;
    }

    // Get the dialogue directory for a specific day
    DialogueDirectory GetDialogueDirectoryForDay(int day)
    {
        foreach (DialogueDirectoryByDay dayDir in dialoguesByDay)
        {
            if (dayDir.day == day && dayDir.directory != null)
            {
                return dayDir.directory;
            }
        }
        return null;
    }

    // Helper method to get current progression directory
    public DialogueDirectory GetCurrentProgressionDirectory()
    {
        return GetDialogueDirectoryForProgression();
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

    // Get the day the NPC was first interacted with
    public int GetFirstInteractionDay()
    {
        return firstInteractionDay;
    }
}

