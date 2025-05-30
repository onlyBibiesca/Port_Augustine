using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    public List<Trait> activeTraits = new List<Trait>();

    public bool HasTraitKeyword(string keyword)
    {
        foreach (var trait in activeTraits)
        {
            if (trait.unlockableKeywords.Contains(keyword))
                return true;
        }
        return false;
    }

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

    
}