using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance;

    [Header("Time Settings")]
    public int currentHour = 8;
    public int currentMinute = 0;
    public int currentDay = 1;

    [Header("Time Display")]
    public TMPro.TMP_Text timeDisplayText;
    public bool use24HourFormat = false;
    public TMPro.TMP_Text dayDisplayText;

    [Header("Debug")]
    public bool showDebugMessages = true;

    public event Action<int, int> OnTimeChanged;
    public event Action<int> OnDayChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Apply wake-up time modifier from traits
        if (TraitsManager.Instance != null)
        {
            currentHour += TraitsManager.Instance.GetWakeUpHourModifier();
            currentHour = Mathf.Clamp(currentHour, 0, 23);

            if (showDebugMessages)
            {
                Debug.Log($"Wake-up modifier applied. Starting hour: {currentHour}");
            }
        }

        UpdateTimeDisplay();
    }

    public void AddTime(int hours, int minutes)
    {
        if (showDebugMessages)
            Debug.Log($"Adding time: {hours}h {minutes}m");

        currentMinute += minutes;
        currentHour += hours;

        // Handle minute overflow
        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
        }

        // Handle hour overflow (new day)
        while (currentHour >= 24)
        {
            currentHour -= 24;
            currentDay++;
            OnDayChanged?.Invoke(currentDay);
            SleepSystem.Instance.MidnightForceSleep();
            UpdateTimeDisplay();
            if (showDebugMessages)
                Debug.Log($"New day! Day {currentDay}");
        }

        UpdateTimeDisplay();

        OnTimeChanged?.Invoke(currentHour, currentMinute);


        if (showDebugMessages)
            Debug.Log($"Current time: {GetFormattedTime()}");
    }

    public void ConsumeTime(ITimeConsumer consumer)
    {
        if (consumer == null)
        {
            Debug.LogError("Cannot consume time: consumer is null!");
            return;
        }

        if (!consumer.ConsumesTime)
        {
            if (showDebugMessages)
                Debug.Log($"{consumer.GetConsumerName()} does not consume time.");
            return;
        }

        int hours = consumer.HoursToConsume;
        int minutes = consumer.MinutesToConsume;

        if (TraitsManager.Instance != null)
        {
            hours += TraitsManager.Instance.GetMovementTimeModifier();
        }

        AddTime(hours, minutes);

        if (showDebugMessages)
            Debug.Log($"✓ {consumer.GetConsumerName()} consumed {hours}h {minutes}m");
    }

    public void SetTime(int hour, int minute)
    {
        currentHour = Mathf.Clamp(hour, 0, 23);
        currentMinute = Mathf.Clamp(minute, 0, 59);

        UpdateTimeDisplay();
        OnTimeChanged?.Invoke(currentHour, currentMinute);

        if (showDebugMessages)
            Debug.Log($"Time set to: {GetFormattedTime()}");
    }

    public void SetDay(int day)
    {
        currentDay = Mathf.Max(1, day);
        OnDayChanged?.Invoke(currentDay);

        UpdateTimeDisplay();

        if (showDebugMessages)
            Debug.Log($"Day set to: {currentDay}");
    }

    void UpdateTimeDisplay()
    {
        if (timeDisplayText != null)
        {
            timeDisplayText.text = GetFormattedTime();
        }
        if (dayDisplayText != null)
        {
            dayDisplayText.text = $"Day {currentDay}";
        }
    }

    public string GetFormattedTime()
    {
        if (use24HourFormat)
        {
            return $"{currentHour:00}:{currentMinute:00}";
        }
        else
        {
            int displayHour = currentHour;
            string period = "AM";

            if (currentHour >= 12)
            {
                period = "PM";
                if (currentHour > 12)
                    displayHour = currentHour - 12;
            }

            if (displayHour == 0)
                displayHour = 12;

            return $"{displayHour}:{currentMinute:00} {period}";
        }
    }

    public int GetTotalMinutes()
    {
        return (currentDay - 1) * 24 * 60 + currentHour * 60 + currentMinute;
    }
}