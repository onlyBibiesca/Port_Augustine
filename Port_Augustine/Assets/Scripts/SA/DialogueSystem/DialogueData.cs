using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue_Choice
{
    [TextArea(2, 5)]
    public string choiceText;
    public Dialogue nextDialogue; // The dialogue to play if this choice is selected

    [Header("Trait Requirements")]
    public TraitSO requiredTrait;

    [Header("Trait Rewards")]
    public TraitSO gainedTrait;

    [Header("Visibility")]
    public bool hideIfTraitMissing = true;
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
public class Dialogue : ScriptableObject
{
    public string dialogueName;
    public DialogueLine[] lines;
}