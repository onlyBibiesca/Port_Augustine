using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEventManager : MonoBehaviour
{
    public static NPCEventManager Instance;

    [Header("Debug")]
    public bool showDebugMessages = true;

    public Dictionary<string, bool> completedEvents = new Dictionary<string, bool>();
    private Dictionary<string, int> eventScores = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Check if NPC has an available event based on relationship
    public NPCEvent GetAvailableEvent(string npcName, int currentRelationship, List<NPCEvent> availableEvents)
    {
        foreach (NPCEvent evt in availableEvents)
        {
            // Check if relationship threshold is met
            if (currentRelationship >= evt.relationshipThreshold)
            {
                // Check if event hasn't been completed yet
                string eventKey = $"{npcName}_{evt.eventName}";
                if (!completedEvents.ContainsKey(eventKey) || !completedEvents[eventKey])
                {
                    if (showDebugMessages)
                        Debug.Log($"Available event for {npcName}: {evt.eventName}");
                    return evt;
                }
            }
        }

        return null;
    }

    // Start tracking event choices
    public void StartEventTracking(string eventKey)
    {
        if (!eventScores.ContainsKey(eventKey))
        {
            eventScores[eventKey] = 0;
        }
        else
        {
            eventScores[eventKey] = 0;
        }

        if (showDebugMessages)
            Debug.Log($"Starting event tracking for: {eventKey}");
    }

    // Record a choice during event
    public void RecordEventChoice(string eventKey, ChoiceType choiceType)
    {
        if (!eventScores.ContainsKey(eventKey))
        {
            eventScores[eventKey] = 0;
        }

        eventScores[eventKey] += (int)choiceType;

        if (showDebugMessages)
            Debug.Log($"Choice recorded: {choiceType}. Event score: {eventScores[eventKey]}");
    }

    public bool IsEventCompleted(string eventKey)
    {
        return completedEvents.ContainsKey(eventKey) && completedEvents[eventKey];
    }

    // Determine if event is successful
    public bool IsEventSuccessful(string eventKey)
    {
        if (!eventScores.ContainsKey(eventKey))
            return false;

        bool success = eventScores[eventKey] > 0;

        if (showDebugMessages)
            Debug.Log($"Event {eventKey} result: {(success ? "SUCCESS" : "FAILURE")} (Score: {eventScores[eventKey]})");

        return success;
    }

    // Mark event as completed
    public void CompleteEvent(string eventKey, bool success)
    {
        completedEvents[eventKey] = success;

        if (showDebugMessages)
            Debug.Log($"Event {eventKey} completed: {(success ? "Success" : "Failure")}");
    }

    // Get event score
    public int GetEventScore(string eventKey)
    {
        return eventScores.ContainsKey(eventKey) ? eventScores[eventKey] : 0;
    }

    // Reset event score
    public void ResetEventScore(string eventKey)
    {
        eventScores[eventKey] = 0;
    }
}
