using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class TraitManager : MonoBehaviour
{
    [Header("Traits the player currently has")]
    public List<Trait> activeTraits = new List<Trait>();

    [Header("Traits available in the game")]
    public List<Trait> availableTraits = new List<Trait>();

    [Header("Gameplay References")]
    public CinemachineVirtualCamera virtualCamera;
    public PlayerMovement playerMovement;

    private float baseZoomSize = 5f; // Default orthographic size (can set in inspector)

    void Start()
    {
        AssignRandomTrait();
        RecalculateGameplayModifiers();
    }

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

    // Apply trait modifiers to the player stats
    public void ApplyTraitModifiers(PlayerManager player)
    {
        foreach (var trait in activeTraits)
        {
            player.ChangeHealth(trait.healthModifier);
            player.ChangeHunger(trait.hungerModifier);
            player.ChangeEnergy(trait.energyModifier);
            player.ChangeSocialBattery(trait.socialModifier);
        }

        RecalculateGameplayModifiers(); // Also apply camera & movement changes
    }

    // Recalculate zoom and speed modifiers
    private void RecalculateGameplayModifiers()
    {
        float zoomOffset = 0f;
        float speedOffset = 0f;

        foreach (var trait in activeTraits)
        {
            zoomOffset += trait.cameraZoomOffset;
            speedOffset += trait.moveSpeedModifier;
        }

        if (virtualCamera != null)
            virtualCamera.m_Lens.OrthographicSize = baseZoomSize + zoomOffset;

        if (playerMovement != null)
            playerMovement.moveSpeedModifier = speedOffset;
    }

    // Give a trait to the player
    public void AddTrait(Trait trait)
    {
        if (!activeTraits.Contains(trait))
        {
            activeTraits.Add(trait);
            RecalculateGameplayModifiers();
            Debug.Log($"Added trait: {trait.traitName}");
        }
    }

    // Remove a trait
    public void RemoveTrait(Trait trait)
    {
        if (activeTraits.Contains(trait))
        {
            activeTraits.Remove(trait);
            RecalculateGameplayModifiers();
            Debug.Log($"Removed trait: {trait.traitName}");
        }
    }

    // Remove all traits
    public void ClearAllTraits()
    {
        activeTraits.Clear();
        RecalculateGameplayModifiers();
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

    // Assign a random trait from the available list
    public void AssignRandomTrait()
    {
        if (availableTraits.Count == 0)
        {
            Debug.LogWarning("No traits available to assign.");
            return;
        }

        Trait randomTrait = availableTraits[UnityEngine.Random.Range(0, availableTraits.Count)];

        if (!activeTraits.Contains(randomTrait))
        {
            activeTraits.Add(randomTrait);
            Debug.Log($"Randomly assigned trait: {randomTrait.traitName}");
            RecalculateGameplayModifiers();
        }
    }
}
