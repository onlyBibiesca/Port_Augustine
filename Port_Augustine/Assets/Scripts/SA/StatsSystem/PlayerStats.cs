using System;

using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Current Stats")]
    public int hunger = 50;
    public int energy = 80;
    public int happiness = 70;

    [Header("Base Stat Limits")]
    public int minStat = 0;

    public int baseMaxHunger = 100;
    public int baseMaxEnergy = 100;
    public int baseMaxHappiness = 100;

    [Header("UI References")]
    public Slider hungerSlider;
    public Slider energySlider;
    public Slider happinessSlider;

    [Header("Debug")]
    public bool showDebugMessages = true;

    public event Action<int> OnHungerChanged;
    public event Action<int> OnEnergyChanged;
    public event Action<int> OnHappinessChanged;

    public int MaxHunger
    {
        get
        {
            if (TraitsManager.Instance == null)
                return baseMaxHunger;

            return baseMaxHunger +
                   TraitsManager.Instance.GetMaxHungerModifier();
        }
    }

    public int MaxEnergy
    {
        get
        {
            if (TraitsManager.Instance == null)
                return baseMaxEnergy;

            return baseMaxEnergy +
                   TraitsManager.Instance.GetMaxEnergyModifier();
        }
    }

    public int MaxHappiness
    {
        get
        {
            if (TraitsManager.Instance == null)
                return baseMaxHappiness;

            return baseMaxHappiness +
                   TraitsManager.Instance.GetMaxHappinessModifier();
        }
    }

    private void Awake()
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

    private void Start()
    {
        if (TraitsManager.Instance != null)
        {
            TraitsManager.Instance.OnTraitsChanged += RefreshStatLimits;
        }

        RefreshStatLimits();
    }

    private void OnDestroy()
    {
        if (TraitsManager.Instance != null)
        {
            TraitsManager.Instance.OnTraitsChanged -= RefreshStatLimits;
        }
    }

    private void RefreshStatLimits()
    {
        hunger = Mathf.Clamp(hunger, minStat, MaxHunger);
        energy = Mathf.Clamp(energy, minStat, MaxEnergy);
        happiness = Mathf.Clamp(happiness, minStat, MaxHappiness);

        UpdateStatsDisplay();
    }

    // Modular stat consumption method
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

        if (consumer.HungerChange != 0)
            ChangeHunger(consumer.HungerChange);

        if (consumer.EnergyChange != 0)
            ChangeEnergy(consumer.EnergyChange);

        if (consumer.HappinessChange != 0)
            ChangeHappiness(consumer.HappinessChange);

        if (showDebugMessages)
            Debug.Log($"{consumer.GetConsumerName()} applied stat changes");
    }

    public void ChangeHunger(int amount)
    {
        hunger = Mathf.Clamp(
            hunger + amount,
            minStat,
            MaxHunger);

        UpdateStatsDisplay();

        OnHungerChanged?.Invoke(hunger);

        if (showDebugMessages)
            Debug.Log($"Hunger changed by {amount}. Current: {hunger}");
    }

    public void ChangeEnergy(int amount)
    {
        energy = Mathf.Clamp(
            energy + amount,
            minStat,
            MaxEnergy);

        UpdateStatsDisplay();

        OnEnergyChanged?.Invoke(energy);

        if (showDebugMessages)
            Debug.Log($"Energy changed by {amount}. Current: {energy}");
    }

    public void ChangeHappiness(int amount)
    {
        happiness = Mathf.Clamp(
            happiness + amount,
            minStat,
            MaxHappiness);

        UpdateStatsDisplay();

        OnHappinessChanged?.Invoke(happiness);

        if (showDebugMessages)
            Debug.Log($"Happiness changed by {amount}. Current: {happiness}");
    }

    public void SetHunger(int value)
    {
        hunger = Mathf.Clamp(
            value,
            minStat,
            MaxHunger);

        UpdateStatsDisplay();

        OnHungerChanged?.Invoke(hunger);
    }

    public void SetEnergy(int value)
    {
        energy = Mathf.Clamp(
            value,
            minStat,
            MaxEnergy);

        UpdateStatsDisplay();

        OnEnergyChanged?.Invoke(energy);
    }

    public void SetHappiness(int value)
    {
        happiness = Mathf.Clamp(
            value,
            minStat,
            MaxHappiness);

        UpdateStatsDisplay();

        OnHappinessChanged?.Invoke(happiness);
    }

    private void UpdateStatsDisplay()
    {
        if (hungerSlider != null)
        {
            hungerSlider.minValue = minStat;
            hungerSlider.maxValue = MaxHunger;
            hungerSlider.value = hunger;
        }

        if (energySlider != null)
        {
            energySlider.minValue = minStat;
            energySlider.maxValue = MaxEnergy;
            energySlider.value = energy;
        }

        if (happinessSlider != null)
        {
            happinessSlider.minValue = minStat;
            happinessSlider.maxValue = MaxHappiness;
            happinessSlider.value = happiness;
        }
    }

    public string GetFormattedStats()
    {
        return $"Hunger: {hunger}/{MaxHunger} | Energy: {energy}/{MaxEnergy} | Happiness: {happiness}/{MaxHappiness}";
    }

    public bool IsCritical()
    {
        return hunger <= 10 ||
               energy <= 10 ||
               happiness <= 10;
    }
}