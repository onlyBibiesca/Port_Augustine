using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    [SerializeField] private TimeScheduleSO scheduleData;
    [SerializeField] private TimeManager timeManager;
    private List<TimeScheduleSO.TimeEvent> triggeredToday = new();

    void Update()
    {
        int currentHour = timeManager.currentHour;

        foreach (var timeEvent in scheduleData.schedule)
        {
            if (timeEvent.hour == currentHour && !triggeredToday.Contains(timeEvent))
            {
                TriggerEvent(timeEvent);
                triggeredToday.Add(timeEvent);
            }
        }

        if (currentHour == 0)
        {
            triggeredToday.Clear(); // Reset for new day
        }
    }

    void TriggerEvent(TimeScheduleSO.TimeEvent timeEvent)
    {
        Debug.Log($"[Schedule] {timeEvent.description} at {timeEvent.hour}:00");

        switch (timeEvent.eventType)
        {
            case ScheduledEventType.CharacterAppear:
                GameObject target = FindObjectByID(timeEvent.targetObjectID);
                if (target != null)
                    target.SetActive(true);
                else
                    Debug.LogWarning($"Target '{timeEvent.targetObjectID}' not found.");
                break;

            case ScheduledEventType.SceneDarken:
                FindObjectOfType<MoodManager>()?.SetNightLighting();
                break;

            case ScheduledEventType.Custom:
                // Placeholder for more stuff
                break;
        }
    }

    GameObject FindObjectByID(string id)
    {
        ScheduledObject[] all = GameObject.FindObjectsOfType<ScheduledObject>(true);
        foreach (var obj in all)
        {
            if (obj.objectID == id)
                return obj.gameObject;
        }
        return null;
    }
}