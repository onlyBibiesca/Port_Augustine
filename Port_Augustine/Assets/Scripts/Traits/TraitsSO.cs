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
    public List<string> unlockableKeywords; // e.g. "sarcastic", "athletic", etc.

    // Modifiers that affect gameplay
    public float cameraZoomOffset; // e.g., nearsighted could be -2
    public float moveSpeedModifier;
}