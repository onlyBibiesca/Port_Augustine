using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatConsumer
{
    bool AffectsStats { get; }
    int HungerChange { get; }
    int EnergyChange { get; }
    int HappinessChange { get; }
    string GetConsumerName();
}


[CreateAssetMenu(fileName = "New Stat Consumable", menuName = "Stats System/Stat Consumable")]
public class StatConsumable : ScriptableObject, IStatConsumer
{
    [Header("Stat Changes")]
    public bool affectsStats = true;

    [Range(-100, 100)]
    public int hungerChange = 0;

    [Range(-100, 100)]
    public int energyChange = 0;

    [Range(-100, 100)]
    public int happinessChange = 0;

    [Header("Info")]
    public string consumableName;
    [TextArea(2, 4)]
    public string description;

    public bool AffectsStats => affectsStats;
    public int HungerChange => hungerChange;
    public int EnergyChange => energyChange;
    public int HappinessChange => happinessChange;
    public string GetConsumerName() => consumableName;
}
