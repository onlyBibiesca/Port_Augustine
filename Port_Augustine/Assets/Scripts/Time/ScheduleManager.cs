using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    [SerializeField] private List<TimeScheduleSO> dailySchedules; // One SO per day
    [SerializeField] private TimeManager timeManager;

    private List<TimeScheduleSO.TimeEvent> activeSchedule; // Deep copied version
    private HashSet<string> triggeredToday = new(); // Track using string key

    private void Start()
    {
        UpdateCurrentSchedule(timeManager.TotalDaysPassed); // Day 0 = Day1Sched
        timeManager.OnDayChanged += OnNewDay;
    }

    private void Update()
    {
        if (activeSchedule == null) return;

        int currentHour = timeManager.currentHour;

        foreach (var timeEvent in activeSchedule)
        {
            string eventKey = $"{timeEvent.hour}|{timeEvent.eventType}|{timeEvent.description}";
            if (timeEvent.hour == currentHour && !triggeredToday.Contains(eventKey))
            {
                TriggerEvent(timeEvent);
                triggeredToday.Add(eventKey);
            }
        }

        if (currentHour == 0)
        {
            triggeredToday.Clear();
        }
    }

    private GameObject FindObjectByID(string id)
    {
        ScheduledObject[] allObjects = Resources.FindObjectsOfTypeAll<ScheduledObject>();
        foreach (var obj in allObjects)
        {
            if (obj.objectID == id)
                return obj.gameObject;
        }

        Debug.LogWarning($"[ScheduleManager] No object found with ID: {id}");
        return null;
    }

    void TriggerEvent(TimeScheduleSO.TimeEvent timeEvent)
    {
        Debug.Log($"[Schedule] {timeEvent.description} at {timeEvent.hour}:00");

        GameObject target = null;

        if (timeEvent.eventType == ScheduledEventType.ShowObject ||
            timeEvent.eventType == ScheduledEventType.HideObject)
        {
            target = FindObjectByID(timeEvent.objectID);
            if (target == null)
            {
                Debug.LogWarning($"[Schedule] Target object not found for ID: {timeEvent.objectID}");
                return;
            }
        }

        switch (timeEvent.eventType)
        {
            case ScheduledEventType.ShowObject:
                target?.SetActive(true);
                break;

            case ScheduledEventType.HideObject:
                target?.SetActive(false);
                break;

            case ScheduledEventType.SceneChangeToMorning:
                FindObjectOfType<MoodManager>()?.SetMorningMood();
                break;

            case ScheduledEventType.SceneChangeToAfternoon:
                FindObjectOfType<MoodManager>()?.SetAfternoonMood();
                break;

            case ScheduledEventType.SceneChangeToNight:
                FindObjectOfType<MoodManager>()?.SetNightMood();
                break;

            case ScheduledEventType.Custom:
                // Handle your custom event type here if needed
                break;
        }
    }

    private void OnNewDay(GameDay day, int totalDays)
    {
        triggeredToday.Clear();
        UpdateCurrentSchedule(totalDays);
    }

    private void UpdateCurrentSchedule(int totalDaysPassed)
    {
        if (dailySchedules.Count == 0)
        {
            Debug.LogWarning("[ScheduleManager] No schedules assigned.");
            return;
        }

        int index = Mathf.Clamp(totalDaysPassed, 0, dailySchedules.Count - 1);
        TimeScheduleSO selectedSchedule = dailySchedules[index];

        // Deep copy the schedule list to avoid shared references
        activeSchedule = new List<TimeScheduleSO.TimeEvent>();
        foreach (var e in selectedSchedule.schedule)
        {
            TimeScheduleSO.TimeEvent copy = new TimeScheduleSO.TimeEvent
            {
                eventName = e.eventName,
                hour = e.hour,
                description = e.description,
                eventType = e.eventType,
                objectID = e.objectID
            };
            activeSchedule.Add(copy);
        }

        Debug.Log($"[ScheduleManager] Loaded schedule for day {totalDaysPassed}: {selectedSchedule.name}");
        foreach (var e in activeSchedule)
        {
            Debug.Log($"[ScheduleManager] Event: {e.description} at {e.hour}:00 | type: {e.eventType} | ID: {e.objectID}");
        }
    }
}