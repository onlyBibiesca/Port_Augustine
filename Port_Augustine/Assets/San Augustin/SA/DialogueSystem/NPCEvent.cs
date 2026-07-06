using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCEvent
{
    public int relationshipThreshold = 15; // Event triggers at this relationship level
    public string eventName = "Special Event";

    [Header("Description")]
    [TextArea(2, 4)]
    public string eventDescription;

    [Header("Event Dialogue")]
    public DialogueDirectory eventDialogueDirectory;
    public List<string> eventDialogueSequence = new List<string>();

    [Header("Event Location")]
    public Transform eventLocation; // Where NPC moves for event

    [Header("Success Outcome")]
    public Dialogue successDialogue;
    public int successRelationshipBonus = 20;

    [Header("Failure Outcome")]
    public Dialogue failureDialogue;
    public int failureRelationshipPenalty = -10;

    [Header("Time Range (Optional)")]
    public bool hasTimeRange = false; // Enable/disable time restriction
    public int startHour = 10; // Event starts at this hour
    public int endHour = 20; // Event ends at this hour
}
