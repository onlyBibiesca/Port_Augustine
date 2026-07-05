using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCEvent
{
    public int relationshipThreshold = 15; // Event triggers at this relationship level
    public string eventName = "Special Event";

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
}   
