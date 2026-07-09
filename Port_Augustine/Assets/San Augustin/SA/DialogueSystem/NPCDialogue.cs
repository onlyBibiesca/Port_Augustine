using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [Header("NPC Identity")]
    public string npcName = "NPC";

    [Header("Dialogue Setup - By Day")]
    [SerializeField] private List<DialogueDirectoryByDay> dialoguesByDay = new List<DialogueDirectoryByDay>();

    [SerializeField] private List<DialogueSequenceItem> dialogueSequence = new List<DialogueSequenceItem>();

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

    [System.Serializable]
    public class DialogueSequenceItem
    {
        public int day = 1;
        public string dialogueName = "";
    }
    void Start()
    {
        originalPosition = transform.position;
        CheckForAvailableEvents();

        if (string.IsNullOrEmpty(npcName))
            Debug.LogError($"NPCDialogue on {gameObject.name} has no NPC name assigned!");

        if (dialoguesByDay.Count == 0)
            Debug.LogWarning($"NPCDialogue on {gameObject.name} has no dialogue directories assigned!");

        originalPosition = transform.position;

        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeChanged += (hour, minute) => CheckForAvailableEvents();
            Debug.Log($"{npcName} subscribed to time changes");
        }
    }

    void OnDestroy()
    {

        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeChanged -= (hour, minute) => CheckForAvailableEvents();
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

    public bool IsEventAvailableNow(NPCEvent evt)
    {
        // If event doesn't have time range, it's always available
        if (!evt.hasTimeRange)
        {
            return true;
        }

        // Get current hour
        int currentHour = TimeSystem.Instance.currentHour;

        // Check if current time is within range
        if (evt.startHour <= evt.endHour)
        {
            // Normal range (e.g., 10:00 - 20:00)
            bool inRange = currentHour >= evt.startHour && currentHour < evt.endHour;

            if (showDebugMessages)
                Debug.Log($"Event {evt.eventName}: Current hour {currentHour}, Range {evt.startHour}-{evt.endHour}, In range? {inRange}");

            return inRange;
        }
        else
        {
            // Overnight range (e.g., 20:00 - 08:00, wraps midnight)
            bool inRange = currentHour >= evt.startHour || currentHour < evt.endHour;

            if (showDebugMessages)
                Debug.Log($"Event {evt.eventName}: Current hour {currentHour}, Overnight range {evt.startHour}-{evt.endHour}, In range? {inRange}");

            return inRange;
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
        Debug.Log($"=== CheckForAvailableEvents called for {npcName} ===");

        if (RelationshipManager.Instance == null || NPCEventManager.Instance == null)
        {
            Debug.LogError("RelationshipManager or NPCEventManager is NULL!");
            return;
        }

        if (TimeSystem.Instance == null)
        {
            Debug.LogError("TimeSystem.Instance is NULL!");
            return;
        }

        int currentRelationship = RelationshipManager.Instance.GetRelationship(npcName);
        int currentHour = TimeSystem.Instance.currentHour;
        Debug.Log($"Current hour: {currentHour}");
        Debug.Log($"Current relationship: {currentRelationship}");
        Debug.Log($"Available events count: {availableEvents.Count}");

        NPCEvent availableEvent = null;

        foreach (NPCEvent evt in availableEvents)
        {
            Debug.Log($"\n--- Checking event: {evt.eventName} ---");
            Debug.Log($"  Relationship threshold: {evt.relationshipThreshold}");
            Debug.Log($"  Current relationship: {currentRelationship}");
            Debug.Log($"  Meets threshold? {currentRelationship >= evt.relationshipThreshold}");

            if (currentRelationship >= evt.relationshipThreshold)
            {
                string eventKey = $"{npcName}_{evt.eventName}";
                Debug.Log($"  Event key: {eventKey}");

                // Check if completed
                bool isCompleted = NPCEventManager.Instance.completedEvents.ContainsKey(eventKey) &&
                                  NPCEventManager.Instance.completedEvents[eventKey];
                Debug.Log($"  Already completed? {isCompleted}");

                if (!isCompleted)
                {
                    // Check time range
                    Debug.Log($"  Has time range? {evt.hasTimeRange}");

                    if (evt.hasTimeRange)
                    {
                        Debug.Log($"  Event time range: {evt.startHour}:00 - {evt.endHour}:00");
                        Debug.Log($"  Current hour: {currentHour}");

                        bool inRange = false;
                        if (evt.startHour <= evt.endHour)
                        {
                            inRange = currentHour >= evt.startHour && currentHour < evt.endHour;
                            Debug.Log($"  Normal range check: {currentHour} >= {evt.startHour} && {currentHour} < {evt.endHour} = {inRange}");
                        }
                        else
                        {
                            inRange = currentHour >= evt.startHour || currentHour < evt.endHour;
                            Debug.Log($"  Overnight range check: {currentHour} >= {evt.startHour} || {currentHour} < {evt.endHour} = {inRange}");
                        }

                        if (inRange)
                        {
                            Debug.Log($"  Event is within time range!");
                            availableEvent = evt;
                            break;
                        }
                        else
                        {
                            Debug.Log($"  Event is outside time range");
                        }
                    }
                    else
                    {
                        Debug.Log($"  Event has no time restriction");
                        availableEvent = evt;
                        break;
                    }
                }
            }
        }

        if (availableEvent != null && currentEvent == null)
        {
            Debug.Log($"\n Event available! Setting up: {availableEvent.eventName}");
            currentEvent = availableEvent;
            currentEventKey = $"{npcName}_{availableEvent.eventName}";

            // Move NPC to event location
            if (availableEvent.eventLocation != null)
            {
                transform.position = availableEvent.eventLocation.position;
                Debug.Log($"NPC moved to event location");
            }

            // Show exclamation mark
            NPCEventIndicator indicator = GetComponent<NPCEventIndicator>();
            if (indicator != null)
            {
                indicator.ShowEventIndicator();
                Debug.Log($"Exclamation mark shown");
            }

            // Show quest in quest panel
            if (QuestPanel.Instance != null)
            {
                QuestPanel.Instance.AddQuest(currentEventKey, availableEvent.eventName, availableEvent.eventDescription);
                Debug.Log($"Quest added to panel");
            }
        }
        else
        {
            Debug.Log($"No available event found or event already active");
        }
    }


    public void OnInteract()
    {
        if (PlayerStats.Instance.energy >= minimumEnergy)
        {
            buttonSound.Play();
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                if (showDebugMessages)
                    Debug.Log($"Cannot interact with {gameObject.name} - dialogue already active!");
                return;
            }

            // IF EVENT IS AVAILABLE, TRIGGER IT INSTEAD
            if (currentEvent != null)
            {
                if (showDebugMessages)
                    Debug.Log($" Triggering event: {currentEvent.eventName}");

                NPCEventManager.Instance.StartEventTracking(currentEventKey);

                if (currentEvent.eventDialogueDirectory != null && currentEvent.eventDialogueSequence.Count > 0)
                {
                    string firstEventDialogue = currentEvent.eventDialogueSequence[0];
                    Dialogue eventDialogue = currentEvent.eventDialogueDirectory.GetDialogueByName(firstEventDialogue);

                    if (eventDialogue != null)
                    {
                        DialogueManager.Instance.StartEventDialogue(eventDialogue, OnEventDialogueFinished, npcName, currentEventKey);
                        return;
                    }
                }

                Debug.LogWarning("Event dialogue not found!");
                return;
            }

            // NORMAL DIALOGUE INTERACTION
            int currentGameDay = TimeSystem.Instance.currentDay;

            // Track first interaction day
            if (firstInteractionDay == -1)
            {
                firstInteractionDay = currentGameDay;
                if (showDebugMessages)
                    Debug.Log($"{npcName} first interacted on Day {firstInteractionDay}");
            }

            // Calculate days passed since first interaction
            int daysSinceFirstInteraction = currentGameDay - firstInteractionDay;

            // Calculate which day's dialogues to play
            int currentProgressionDay = fallbackDay + daysSinceFirstInteraction;

            if (showDebugMessages)
                Debug.Log($"Days passed: {daysSinceFirstInteraction}, Playing dialogues for Day {currentProgressionDay}");

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

            // Find and play the first dialogue of the current progression day
            PlayDialogueForDay(currentProgressionDay);
        }

        // NEW METHOD: Play dialogue for a specific day
        void PlayDialogueForDay(int targetDay)
        {
            // Find the first dialogue in the sequence that matches targetDay
            for (int i = currentDialogueIndex; i < dialogueSequence.Count; i++)
            {
                DialogueSequenceItem sequenceItem = dialogueSequence[i];

                if (sequenceItem.day == targetDay)
                {
                    // Found a dialogue for this day
                    currentDialogueIndex = i;
                    PlayCurrentDialogue(null);
                    return;
                }
            }

            // No dialogue found for this day, might be playing future content
            if (showDebugMessages)
                Debug.Log($"No dialogue available for Day {targetDay} yet");

        }
    }
    void OnEventDialogueFinished()
    {
        Debug.Log($"Event dialogue finished");

        bool eventSuccess = NPCEventManager.Instance.IsEventSuccessful(currentEventKey);
        Debug.Log($"Event success? {eventSuccess}");

        if (eventSuccess)
        {
            Debug.Log("Playing SUCCESS dialogue");
            if (currentEvent.successDialogue != null)
                DialogueManager.Instance.StartDialogue(currentEvent.successDialogue, OnEventOutcomeFinished, npcName);
        }
        else
        {
            Debug.Log("Playing FAILURE dialogue");
            if (currentEvent.failureDialogue != null)
                DialogueManager.Instance.StartDialogue(currentEvent.failureDialogue, OnEventOutcomeFinished, npcName);
        }

        NPCEventManager.Instance.CompleteEvent(currentEventKey, eventSuccess);
    }

    void OnEventOutcomeFinished()
    {
        // Remove quest from panel
        if (QuestPanel.Instance != null && !string.IsNullOrEmpty(currentEventKey))
        {
            QuestPanel.Instance.RemoveQuest(currentEventKey);
            if (showDebugMessages)
                Debug.Log($"Quest removed from panel: {currentEventKey}");
        }

        Debug.Log($" === OnEventOutcomeFinished called ===");

        // Reset event tracking
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.isInEvent = false;
            DialogueManager.Instance.currentEventKey = "";
        }

        // Return NPC to original position
        transform.position = originalPosition;

        // Hide indicator
        NPCEventIndicator indicator = GetComponent<NPCEventIndicator>();
        if (indicator != null)
            indicator.HideEventIndicator();

        // Clear event
        currentEvent = null;
        currentEventKey = "";

        if (showDebugMessages)
            Debug.Log($" Event completed. {npcName} returned to original position");
    }

    void PlayCurrentDialogue(DialogueDirectory currentDirectory)
    {
        if (currentDialogueIndex >= dialogueSequence.Count)
        {
            if (showDebugMessages)
                Debug.Log("All dialogues completed!");
            return;
        }

        // Get the dialogue sequence item with day info
        DialogueSequenceItem sequenceItem = dialogueSequence[currentDialogueIndex];

        // Get the correct directory for this dialogue's day
        DialogueDirectory correctDirectory = GetDialogueDirectoryForDay(sequenceItem.day);

        if (correctDirectory == null)
        {
            Debug.LogError($"No dialogue directory found for day {sequenceItem.day}!");
            return;
        }

        // Get the dialogue
        Dialogue dialogue = correctDirectory.GetDialogueByName(sequenceItem.dialogueName);

        if (dialogue != null)
        {
            if (showDebugMessages)
                Debug.Log($"Playing dialogue: {sequenceItem.dialogueName} (Day {sequenceItem.day})");

            DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished, npcName);
        }
        else
        {
            Debug.LogError($"Dialogue '{sequenceItem.dialogueName}' not found in day {sequenceItem.day} directory!");
        }
    }

    void OnDialogueFinished()
    {
        Debug.Log($" === OnDialogueFinished CALLED for {npcName} ===");

        if (showDebugMessages)
            Debug.Log($"Dialogue finished. Moving to next dialogue. Current index: {currentDialogueIndex}");

        currentDialogueIndex++;

        // CHECK FOR AVAILABLE EVENTS AFTER DIALOGUE FINISHES
        CheckForAvailableEvents();
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

