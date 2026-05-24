using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitCategory
{
    Random,
    Origin,
    Relationship,
    Job,
    Lifestyle,
    Personality
}

[CreateAssetMenu(fileName = "New Trait", menuName = "Traits/Trait")]
public class TraitSO : ScriptableObject
{
    [Header("Info")]
    public string traitName;

    [TextArea]
    public string description;

    public TraitCategory category;

    [Header("Future")]
    public bool affectsGameplay;
}