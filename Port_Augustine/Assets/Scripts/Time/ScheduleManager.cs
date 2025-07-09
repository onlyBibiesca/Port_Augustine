using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    [SerializeField] private TimeScheduleSO scheduleData;
    [SerializeField] private TimeManager timeManager;

    private HashSet<string> triggeredToday = new HashSet<string>(); // Use eventName as key

    private void Update()
    {
        int currentHour = timeManager.currentHour; // Access the property directly

        foreach (var timeEvent in scheduleData.schedule)
        {
            if (timeEvent.hour == currentHour && !triggeredToday.Contains(timeEvent.eventName))
            {
                Debug.Log($"{timeEvent.description} ({timeEvent.eventName}) at {timeEvent.hour}:00");
                triggeredToday.Add(timeEvent.eventName);
            }
        }

        // Reset only once per day (when it's 12:00 AM)
        if (currentHour == 0 && triggeredToday.Count > 0)
        {
            triggeredToday.Clear();
            Debug.Log("Schedule reset for new day.");
        }
    }
}