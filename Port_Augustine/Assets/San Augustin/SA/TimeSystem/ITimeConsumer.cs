using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeConsumer
{
    bool ConsumesTime { get; }
    int HoursToConsume { get; }
    int MinutesToConsume { get; }
    string GetConsumerName();
}

