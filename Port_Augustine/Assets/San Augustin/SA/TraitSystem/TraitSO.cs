using System.Collections.Generic;
using UnityEngine;

public enum TraitCategory
{
    
    Origin,
    Relationship,
    Job,
    Personality
}

[CreateAssetMenu(fileName = "New Trait", menuName = "Traits/Trait")]
public class TraitSO : ScriptableObject
{
    [Header("Basic Info")]
    public string traitName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Testing")]
    public Sprite originPreviewSprite;

    [Header("Category")]
    public TraitCategory category;

    [Header("Trait Rules")]
    public bool uniqueCategory = false;

    [Header("Time Modifiers")]
    public int movementTimeModifier = 0;

    [Header("Future Gameplay")]
    public float workIncomeMultiplier = 1f;

    [Header("Future World Setup")]
    public string startingBuildingID;

    [Header("Optional")]
    public List<TraitSO> grantedTraits = new();

    [Header("Stat Modifiers")]
    public int hungerMaxBonus = 0;
    public int energyMaxBonus = 0;
    public int happinessMaxBonus = 0;

    [Header("Relationship Modifiers")]
    public int relationshipGainModifier = 0;

}