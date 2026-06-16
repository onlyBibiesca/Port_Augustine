using System.Collections.Generic;
using UnityEngine;

public class TraitsManager : MonoBehaviour
{
    public static TraitsManager Instance;
    public System.Action OnTraitsChanged;

    [SerializeField]
    private List<TraitSO> activeTraits = new();

    public IReadOnlyList<TraitSO> ActiveTraits => activeTraits;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasTrait(TraitSO trait)
    {
        return activeTraits.Contains(trait);
    }

    public void AddTrait(TraitSO trait)
    {
        if (trait == null) return;
        if (HasTrait(trait)) return;

        if (trait.uniqueCategory)
            RemoveTraitsOfCategory(trait.category);

        activeTraits.Add(trait);

        Debug.Log($"Trait Added: {trait.traitName}");

        foreach (var granted in trait.grantedTraits)
            AddTrait(granted);

        OnTraitsChanged?.Invoke();
    }

    public void RemoveTrait(TraitSO trait)
    {
        if (trait == null)
            return;

        activeTraits.Remove(trait);
        OnTraitsChanged?.Invoke();
    }

    public void RemoveTraitsOfCategory(TraitCategory category)
    {
        activeTraits.RemoveAll(t => t.category == category);
        OnTraitsChanged?.Invoke();
    }

    public List<TraitSO> GetTraitsOfCategory(TraitCategory category)
    {
        return activeTraits.FindAll(t => t.category == category);
    }

    public int GetMovementTimeModifier()
    {
        int modifier = 0;

        foreach (var trait in activeTraits)
        {
            modifier += trait.movementTimeModifier;
        }

        return modifier;
    }

    public int GetMaxEnergyModifier()
    {
        int total = 0;

        foreach (var trait in activeTraits)
        {
            total += trait.maxEnergyModifier;
        }

        return total;
    }

    public int GetMaxHungerModifier()
    {
        int total = 0;

        foreach (var trait in activeTraits)
        {
            total += trait.maxHungerModifier;
        }

        return total;
    }

    public int GetMaxHappinessModifier()
    {
        int total = 0;

        foreach (var trait in activeTraits)
        {
            total += trait.maxHappinessModifier;
        }

        return total;
    }
}