using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
