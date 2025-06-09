using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    [Header("Traits the player currently has")]
    public List<Trait> activeTraits = new List<Trait>();

    [Header("Traits available in the game")]
    public List<Trait> availableTraits = new List<Trait>();

    // Check if the player has a trait by keyword
    public bool HasTraitKeyword(string keyword)
    {
        foreach (var trait in activeTraits)
        {
            if (trait.unlockableKeywords.Contains(keyword))
                return true;
        }
        return false;
    }

    // Apply trait modifiers to the player
    public void ApplyTraitModifiers(PlayerManager player)
    {
        foreach (var trait in activeTraits)
        {
            player.ChangeHealth(trait.healthModifier);
            player.ChangeHunger(trait.hungerModifier);
            player.ChangeEnergy(trait.energyModifier);
            player.ChangeSocialBattery(trait.socialModifier);
        }
    }

    // Give a trait to the player
    public void AddTrait(Trait trait)
    {
        if (!activeTraits.Contains(trait))
            activeTraits.Add(trait);
    }

    // Remove all traits
    public void ClearAllTraits()
    {
        activeTraits.Clear();
    }

    public List<string> GetAllTraitNames()
    {
        List<string> names = new List<string>();
        foreach (var trait in activeTraits)
        {
            names.Add(trait.traitName);
        }
        return names;
    }

    public List<Trait> GetAllActiveTraits()
    {
        return activeTraits;
    }
}