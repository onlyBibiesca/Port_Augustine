using UnityEngine;
using System;

[Serializable]
public class GameTimeData
{
    public int hour;
    public int minute;
}

public enum TimePhase
{
    Sunrise,   // 6:00 - 8:00
    Morning,   // 8:10 - 11:50
    Noon,      // 12:00 - 13:50
    Afternoon, // 14:00 - 17:50
    Evening,   // 18:00 - 20:50
    Night,     // 21:00 - 23:50
    LateNight  // 0:00 - 1:50
}

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int startHour = 6;
    public int startMinute = 0;
    public int endHour = 2; // 2 AM = 26:00

    public float realSecondsPerGameStep = 7f; // 10 in-game minutes = 7 real seconds

    public int currentHour { get; private set; }
    public int currentMinute { get; private set; }

    public TimePhase CurrentPhase { get; private set; }
    public bool IsPaused { get; private set; } = false;

    public event Action<string> OnTimeChanged;
    public event Action<TimePhase> OnPhaseChanged;
    public event Action OnDayEnd;

    private float timer;

    void Start()
    {
        LoadTime(); // Or StartNewDay();
    }

    void Update()
    {
        if (IsPaused) return;

        timer += Time.deltaTime;
        if (timer >= realSecondsPerGameStep)
        {
            AdvanceTime(10);
            timer = 0f;
        }
    }

    public void AdvanceTime(int minutesToAdd)
    {
        currentMinute += minutesToAdd;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
        }

        int currentTime24 = (currentHour < 6) ? currentHour + 24 : currentHour;
        int endTime24 = endHour + 24;

        if (currentTime24 >= endTime24)
        {
            OnDayEnd?.Invoke();
            StartNewDay();
            return;
        }

        UpdatePhase();
        NotifyTimeChanged();
    }

    public void SkipTime(int totalMinutes)
    {
        AdvanceTime(totalMinutes);
    }

    public void PauseTime() => IsPaused = true;
    public void ResumeTime() => IsPaused = false;
    public void TogglePause() => IsPaused = !IsPaused;

    public void StartNewDay()
    {
        currentHour = startHour;
        currentMinute = startMinute;
        UpdatePhase();
        NotifyTimeChanged();
    }

    public string GetFormattedTime()
    {
        int displayHour = currentHour > 12 ? currentHour - 12 : currentHour;
        if (displayHour == 0) displayHour = 12;
        string ampm = (currentHour >= 12 && currentHour < 24) ? "PM" : "AM";
        return $"{displayHour}:{currentMinute.ToString("D2")} {ampm}";
    }

    public float GetTimeAsFloat()
    {
        return currentHour + currentMinute / 60f;
    }

    private void NotifyTimeChanged()
    {
        OnTimeChanged?.Invoke(GetFormattedTime());
    }

    // ----------- Time Phases -----------

    private void UpdatePhase()
    {
        TimePhase newPhase = GetPhaseFromTime(currentHour, currentMinute);
        if (newPhase != CurrentPhase)
        {
            CurrentPhase = newPhase;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }

    private TimePhase GetPhaseFromTime(int hour, int minute)
    {
        int time = hour * 100 + minute;

        if (time >= 600 && time <= 800) return TimePhase.Sunrise;
        if (time > 800 && time < 1200) return TimePhase.Morning;
        if (time >= 1200 && time < 1400) return TimePhase.Noon;
        if (time >= 1400 && time < 1800) return TimePhase.Afternoon;
        if (time >= 1800 && time < 2100) return TimePhase.Evening;
        if (time >= 2100 && time < 2400) return TimePhase.Night;
        if (hour >= 0 && hour < 2) return TimePhase.LateNight;

        return TimePhase.Morning; // fallback
    }

    // ----------- Save / Load -----------

    public void SaveTime()
    {
        GameTimeData saveData = new GameTimeData
        {
            hour = currentHour,
            minute = currentMinute
        };
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("GameTime", json);
    }

    public void LoadTime()
    {
        if (PlayerPrefs.HasKey("GameTime"))
        {
            string json = PlayerPrefs.GetString("GameTime");
            GameTimeData data = JsonUtility.FromJson<GameTimeData>(json);
            currentHour = data.hour;
            currentMinute = data.minute;
        }
        else
        {
            StartNewDay();
        }

        UpdatePhase();
        NotifyTimeChanged();
    }

    public void ClearSavedTime()
    {
        PlayerPrefs.DeleteKey("GameTime");
    }
}