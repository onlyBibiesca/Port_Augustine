using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeSchedule", menuName = "Time System/Schedule")]
public class TimeScheduleSO : ScriptableObject
{
    [System.Serializable]
    public class TimeEvent
    {
        public string eventName;
        public int hour; // 24-hour format, e.g., 21 for 9PM
        public string description; // e.g., "Triple 777 closes", "Clerk appears"
    }

    public List<TimeEvent> schedule = new List<TimeEvent>();
}