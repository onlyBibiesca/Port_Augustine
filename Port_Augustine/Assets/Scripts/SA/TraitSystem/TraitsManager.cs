using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitsManager : MonoBehaviour
{
    public static TraitsManager Instance;

    [Header("Current Traits")]
    public List<TraitSO> activeTraits = new List<TraitSO>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool HasTrait(TraitSO trait)
    {
        return activeTraits.Contains(trait);
    }

    public void AddTrait(TraitSO trait)
    {
        if (!activeTraits.Contains(trait))
        {
            activeTraits.Add(trait);
            Debug.Log($"Trait gained: {trait.traitName}");
        }
    }

    public void RemoveTrait(TraitSO trait)
    {
        if (activeTraits.Contains(trait))
        {
            activeTraits.Remove(trait);
            Debug.Log($"Trait removed: {trait.traitName}");
        }
    }
}