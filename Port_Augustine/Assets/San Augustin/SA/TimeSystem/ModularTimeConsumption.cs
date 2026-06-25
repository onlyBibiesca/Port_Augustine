using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Time Consumable", menuName = "Time System/Time Consumable")]
public class TimeConsumable : ScriptableObject, ITimeConsumer
{
    [Header("Time Consumption")]
    public bool consumesTime = true;
    public int hoursToConsume = 1;
    public int minutesToConsume = 0;

    [Header("Info")]
    public string consumableName;
    [TextArea(2, 4)]
    public string description;

    public bool ConsumesTime => consumesTime;
    public int HoursToConsume => hoursToConsume;
    public int MinutesToConsume => minutesToConsume;
    public string GetConsumerName() => consumableName;
}
