using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance;

    [Header("Relationship Settings")]
    [Range(0, 100)]
    public int defaultRelationship = 0;

    [Range(0, 100)]
    public int maxRelationship = 100;

    [Range(0, 100)]
    public int minRelationship = 0;

    [Header("UI References")]
    public UnityEngine.UI.Slider relationshipSlider;
    public TMP_Text relationshipLabel; // Optional - shows NPC name and status

    [Header("Debug")]
    public bool showDebugMessages = true;

    private Dictionary<string, int> npcRelationships = new Dictionary<string, int>();
    public event Action<string, int> OnRelationshipChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("RelationshipManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Get relationship value for an NPC
    public int GetRelationship(string npcName)
    {
        if (npcRelationships.ContainsKey(npcName))
        {
            return npcRelationships[npcName];
        }
        else
        {
            // First time meeting this NPC
            npcRelationships[npcName] = defaultRelationship;
            return defaultRelationship;
        }
    }

    // Change relationship for an NPC
    public void ChangeRelationship(string npcName, int amount)
    {
        int finalAmount = amount;

        // Apply trait modifier only to positive gains
        if (amount > 0 && TraitsManager.Instance != null)
        {
            finalAmount += TraitsManager.Instance.GetRelationshipModifier();

            // Prevent positive gains from becoming negative
            finalAmount = Mathf.Max(0, finalAmount);
        }

        int currentRelationship = GetRelationship(npcName);

        int newRelationship = Mathf.Clamp(
            currentRelationship + finalAmount,
            minRelationship,
            maxRelationship
        );

        npcRelationships[npcName] = newRelationship;

        if (showDebugMessages)
        {
            Debug.Log(
                $"{npcName}: Relationship changed by {finalAmount} " +
                $"(Base: {amount}, Trait Modifier: {finalAmount - amount}) " +
                $"Current: {newRelationship}"
            );
        }

        OnRelationshipChanged?.Invoke(npcName, newRelationship);
        UpdateRelationshipDisplay(npcName, newRelationship);
        DailySummaryManager.Instance?.RecordRelationshipChange(npcName, finalAmount);
    }
    // Set relationship directly
    public void SetRelationship(string npcName, int value)
    {
        int clampedValue = Mathf.Clamp(value, minRelationship, maxRelationship);
        npcRelationships[npcName] = clampedValue;

        if (showDebugMessages)
            Debug.Log($"{npcName}: Relationship set to {clampedValue}");

        OnRelationshipChanged?.Invoke(npcName, clampedValue);
        UpdateRelationshipDisplay(npcName, clampedValue);
    }

    void UpdateRelationshipDisplay(string npcName, int relationshipValue)
    {
        if (relationshipSlider != null)
        {
            relationshipSlider.minValue = minRelationship;
            relationshipSlider.maxValue = maxRelationship;
            relationshipSlider.value = relationshipValue;

            Canvas.ForceUpdateCanvases();

            Debug.Log($"Slider updated to: {relationshipSlider.value}");
        }

        if (relationshipLabel != null)
        {
            string status = GetRelationshipStatus(npcName, relationshipValue);
            relationshipLabel.text = $"{npcName}: {relationshipValue} ({status})";
        }
    }

    // Get relationship status as text
    public string GetRelationshipStatus(string npcName, int value)
    {
        if (value >= 80) return "Loves You";
        if (value >= 50) return "Likes You";
        if (value >= 20) return "Friendly";
        if (value >= 0) return "Neutral";
        if (value >= -30) return "Dislikes You";
        return "Hates You";
    }

    public string GetFormattedRelationship(string npcName)
    {
        int value = GetRelationship(npcName);
        string status = GetRelationshipStatus(npcName, value);
        return $"{npcName}: {value} ({status})";
    }

    // Show relationship slider for an NPC
    public void ShowRelationshipSlider(string npcName)
    {
        int relationshipValue = GetRelationship(npcName);
        UpdateRelationshipDisplay(npcName, relationshipValue);

        if (relationshipSlider != null)
            relationshipSlider.gameObject.SetActive(true);

        if (relationshipLabel != null)
            relationshipLabel.gameObject.SetActive(true);
    }

    // Hide relationship slider
    public void HideRelationshipSlider()
    {
        if (relationshipSlider != null)
            relationshipSlider.gameObject.SetActive(false);

        if (relationshipLabel != null)
            relationshipLabel.gameObject.SetActive(false);
    }

    // Get all relationships
    public Dictionary<string, int> GetAllRelationships()
    {
        return new Dictionary<string, int>(npcRelationships);
    }
}
