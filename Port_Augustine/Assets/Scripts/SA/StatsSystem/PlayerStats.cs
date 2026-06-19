using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stat Settings")]
    [Range(0, 100)]
    public int hunger = 50;

    [Range(0, 100)]
    public int energy = 80;

    [Range(0, 100)]
    public int happiness = 70;

    [Header("Stat Limits")]
    [Range(0, 100)]
    public int minStat = 0;

    [Range(0, 100)]
    public int maxStat = 100;

    private int BaseEnergyMax => 100;

    [Header("UI References")]
    public UnityEngine.UI.Slider hungerSlider;
    public UnityEngine.UI.Slider energySlider;
    public UnityEngine.UI.Slider happinessSlider;

    [Header("Debug")]
    public bool showDebugMessages = true;

    public event Action<int> OnHungerChanged;
    public event Action<int> OnEnergyChanged;
    public event Action<int> OnHappinessChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("PlayerStats initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateStatsDisplay();
    }

    // Modular stat consumption method - works with any IStatConsumer
    public void ConsumeStat(IStatConsumer consumer)
    {
        if (consumer == null)
        {
            Debug.LogError("Cannot consume stats: consumer is null!");
            return;
        }

        if (!consumer.AffectsStats)
        {
            if (showDebugMessages)
                Debug.Log($"{consumer.GetConsumerName()} does not affect stats.");
            return;
        }

        if (showDebugMessages)
            Debug.Log($"=== {consumer.GetConsumerName()} Stat Changes ===");

        // Apply hunger change
        if (consumer.HungerChange != 0)
        {
            ChangeHunger(consumer.HungerChange);
        }

        // Apply energy change
        if (consumer.EnergyChange != 0)
        {
            ChangeEnergy(consumer.EnergyChange);
        }

        // Apply happiness change
        if (consumer.HappinessChange != 0)
        {
            ChangeHappiness(consumer.HappinessChange);
        }

        if (showDebugMessages)
            Debug.Log($" {consumer.GetConsumerName()} applied stat changes");
    }

    public void ChangeHunger(int amount)
    {
        hunger = Mathf.Clamp(hunger + amount, minStat, maxStat);
        UpdateStatsDisplay();
        OnHungerChanged?.Invoke(hunger);

        if (showDebugMessages)
            Debug.Log($"Hunger changed by {amount}. Current: {hunger}");
    }

    public void ChangeEnergy(int amount)
    {
        energy = Mathf.Clamp(energy + amount, minStat, GetMaxEnergy());
        UpdateStatsDisplay();
        OnEnergyChanged?.Invoke(energy);

        if (showDebugMessages)
            Debug.Log($"Energy changed by {amount}. Current: {energy}");
    }

    public void ChangeHappiness(int amount)
    {
        happiness = Mathf.Clamp(happiness + amount, minStat, maxStat);
        UpdateStatsDisplay();
        OnHappinessChanged?.Invoke(happiness);

        if (showDebugMessages)
            Debug.Log($"Happiness changed by {amount}. Current: {happiness}");
    }

    public void SetHunger(int value)
    {
        hunger = Mathf.Clamp(value, minStat, maxStat);
        UpdateStatsDisplay();
        OnHungerChanged?.Invoke(hunger);
    }

    public void SetEnergy(int value)
    {
        energy = Mathf.Clamp(value, minStat, GetMaxEnergy());
        UpdateStatsDisplay();
        OnEnergyChanged?.Invoke(energy);
    }

    public void SetHappiness(int value)
    {
        happiness = Mathf.Clamp(value, minStat, maxStat);
        UpdateStatsDisplay();
        OnHappinessChanged?.Invoke(happiness);
    }

    void UpdateStatsDisplay()
    {
        if (hungerSlider != null)
        {
            hungerSlider.minValue = minStat;
            hungerSlider.maxValue = maxStat;
            hungerSlider.value = hunger;
        }

        if (energySlider != null)
        {
            energySlider.minValue = minStat;
            energySlider.maxValue = GetMaxEnergy();
            energySlider.value = energy;
        }

        if (happinessSlider != null)
        {
            happinessSlider.minValue = minStat;
            happinessSlider.maxValue = maxStat;
            happinessSlider.value = happiness;
        }
    }

    // Get all stats as a formatted string
    public string GetFormattedStats()
    {
        return $"Hunger: {hunger} | Energy: {energy} | Happiness: {happiness}";
    }

    // Check if player is in critical condition
    public bool IsCritical()
    {
        return hunger <= 10 || energy <= 10 || happiness <= 10;
    }

    public int GetMaxEnergy()
    {
        int bonus = 0;

        if (TraitsManager.Instance != null)
        {
            foreach (var trait in TraitsManager.Instance.ActiveTraits)
            {
                bonus += trait.energyMaxBonus;
            }
        }

        return BaseEnergyMax + bonus;
    }

    public void DebugStats()
    {
        Debug.Log($"Hunger: {hunger}/{maxStat} | Energy: {energy}/{GetMaxEnergy()} | Happiness: {happiness}/{maxStat}");
    }
}
