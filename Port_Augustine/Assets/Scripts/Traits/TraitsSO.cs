using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrait", menuName = "Traits/Trait")]
public class Trait : ScriptableObject
{
    public string traitName;
    public string description;

    // Optional modifiers to stats
    public int healthModifier;
    public int hungerModifier;
    public int energyModifier;
    public int socialModifier;

    // Influence on dialogue
    public List<string> unlockableKeywords; // e.g. "sarcastic", "athletic", etc. THIS UNLOCKS DIALOUGES this basically answer the game's question "Can I choose this?"

    // Modifiers that affect gameplay
    public float cameraZoomOffset; // e.g., nearsighted could be -2
    public float moveSpeedModifier;

    public List<DialogueReaction> dialogueReactions; //Per-choice trait reactions
}

[System.Serializable]
public class DialogueReaction
{
    public string choiceType; // e.g., "honest", "sarcastic", "brag" THIS CHANGES YOUR STATS ie this basically answer the game's question "What kind of answer did I choose?"
    public int healthChange;
    public int hungerChange;
    public int energyChange;
    public int socialBatteryChange;
    public int moneyChange;
}