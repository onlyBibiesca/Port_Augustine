using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    [SerializeField] private List<TimeScheduleSO> dailySchedules; // One SO per day
    [SerializeField] private TimeManager timeManager;

    private TimeScheduleSO currentSchedule;
    private List<TimeScheduleSO.TimeEvent> triggeredToday = new();

    private void Start()
    {
        UpdateCurrentSchedule(timeManager.TotalDaysPassed); // e.g., Day 0 = Day1Sched
        timeManager.OnDayChanged += OnNewDay;
    }

    private void Update()
    {
        if (currentSchedule == null) return;

        int currentHour = timeManager.currentHour;

        foreach (var timeEvent in currentSchedule.schedule)
        {
            if (timeEvent.hour == currentHour && !triggeredToday.Contains(timeEvent))
            {
                TriggerEvent(timeEvent);
                triggeredToday.Add(timeEvent);
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

        GameObject target = FindObjectByID(timeEvent.objectID);

        switch (timeEvent.eventType)
        {
            case ScheduledEventType.ShowObject:
                if (target) target.SetActive(true);
                break;

            case ScheduledEventType.HideObject:
                if (target) target.SetActive(false);
                break;

            case ScheduledEventType.SceneDarken:
                FindObjectOfType<MoodManager>()?.SetNightLighting();
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
        currentSchedule = dailySchedules[index];
        Debug.Log($"[ScheduleManager] Loaded schedule for day {totalDaysPassed}: {currentSchedule.name}");
    }
}