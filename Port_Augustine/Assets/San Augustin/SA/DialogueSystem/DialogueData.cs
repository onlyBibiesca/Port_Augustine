using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChoiceType
{
    Neutral = 0,  // No points
    Good = 1,     // +1 point towards success
    Bad = -1      // -1 point towards success
}

[System.Serializable]
public class Dialogue_Choice
{
    [TextArea(2, 5)]
    public string choiceText;
    public Dialogue nextDialogue; // The dialogue to play if this choice is selected

    [Header("Relationship Change")]
    [Range(-100, 100)]
    public int relationshipChange = 0;

    [Header("Event Choice Type")]
    public ChoiceType choiceType = ChoiceType.Neutral;

    [Header("Trait Requirements")]
    public TraitSO requiredTrait;

    [Header("Trait Rewards")]
    public TraitSO gainedTrait;

    [Header("Visibility")]
    public bool hideIfTraitMissing = true;
}

[System.Serializable]
public class NPCEvent
{
    public int relationshipThreshold = 50; // Event triggers at this relationship level
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

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string text;

    [Header("Choices (Optional)")]
    public bool hasChoices = false;
    public Dialogue_Choice[] choices;

    [Header("Character Portrait (Optional)")]
    public Sprite characterPortrait;

}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject, ITimeConsumer
{
    public string dialogueName;
    public DialogueLine[] lines;

    [Header("Time Consumption")]
    public bool consumesTime = true;
    public int hoursToConsume = 1;
    public int minutesToConsume = 0;

    [Header("Stat Consumption")]
    public StatConsumable statConsumable;

    // ITimeConsumer implementation
    public bool ConsumesTime => consumesTime;
    public int HoursToConsume => hoursToConsume;
    public int MinutesToConsume => minutesToConsume;
    public string GetConsumerName() => dialogueName;
}