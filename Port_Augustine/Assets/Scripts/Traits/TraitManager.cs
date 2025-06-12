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

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float baseZoomSize = 4f;

    void Start()
    {
        AssignRandomTrait(); // Ensure trait is added randomly at start
        RecalculateCameraZoom(); // Ensure zoom is updated at start
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

    // Recalculate zoom based on all active traits
    private void RecalculateCameraZoom()
    {
        if (virtualCamera == null)
        {
            Debug.LogWarning("Virtual Camera is not assigned!");
            return;
        }

        float zoomOffset = 0f;
        foreach (var trait in activeTraits)
        {
            zoomOffset += trait.cameraZoomOffset;
        }

        virtualCamera.m_Lens.OrthographicSize = baseZoomSize + zoomOffset;
        Debug.Log($"[TraitManager] Camera zoom set to {virtualCamera.m_Lens.OrthographicSize}");
    }

    // Apply stat modifiers (zoom is handled separately)
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
        {
            activeTraits.Add(trait);
            Debug.Log($"Trait added: {trait.traitName}");
            RecalculateCameraZoom();
        }
    }

    // Remove a trait (Removes the trait's effects ie Zoom)
    public void RemoveTrait(Trait trait)
    {
        if (activeTraits.Contains(trait))
        {
            activeTraits.Remove(trait);
            Debug.Log($"Trait removed: {trait.traitName}");
            RecalculateCameraZoom();
        }
    }

    // Clear all traits (Removes the trait from the player)
    public void ClearAllTraits()
    {
        activeTraits.Clear();
        RecalculateCameraZoom();
        Debug.Log("All traits cleared.");
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

    // Assign one random trait
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
            RecalculateCameraZoom();
        }
    }
}