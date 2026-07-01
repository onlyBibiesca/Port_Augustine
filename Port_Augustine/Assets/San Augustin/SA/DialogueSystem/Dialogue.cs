using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject, ITimeConsumer
{
    public string dialogueName;
    public DialogueLine[] lines;

    [Header("Time Consumption")]
    public bool consumesTime = true;
    public int hoursToConsume = 1;
    public int minutesToConsume = 0;

    [Header("Stat Consumption")]
    public StatConsumable statConsumable;

    // ITimeConsumer implementation
    public bool ConsumesTime => consumesTime;
    public int HoursToConsume => hoursToConsume;
    public int MinutesToConsume => minutesToConsume;
    public string GetConsumerName() => dialogueName;
}