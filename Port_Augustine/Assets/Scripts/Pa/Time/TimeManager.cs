using UnityEngine;
using System;

// ------------------ Serializable Save Class ------------------
[System.Serializable]
public class GameTimeData
{
    public int hour;
    public int minute;
}

// ------------------ Day and TimePhase Enums ------------------
public enum GameDay { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday } // Change to Mon, Tue, Wed etc for Watch 
public enum TimePhase { Sunrise, Morning, Noon, Afternoon, Evening, Night, LateNight }

public class TimeManager : MonoBehaviour
{
    // ------------------ Time Configuration ------------------
    [Header("Time Settings")]
    public int startHour = 6;
    public int startMinute = 0;
    public int endHour = 2; // 2 AM = 26:00
    public float realSecondsPerGameStep = 7f; // Every real 7s = 10 in-game mins

    // ------------------ Current Time State ------------------
    public int currentHour { get; private set; }
    public int currentMinute { get; private set; }
    public TimePhase CurrentPhase { get; private set; }

    // ------------------ Day & Week Tracking ------------------
    public GameDay CurrentDay { get; private set; } = GameDay.Monday;
    public int DayIndex { get; private set; } = 0;
    public int TotalDaysPassed { get; private set; } = 0;

    // ------------------ Time Progression Control ------------------
    public bool IsPaused { get; private set; } = false;
    private float timer;

    // ------------------ Events ------------------
    public event Action<string> OnTimeChanged;
    public event Action<TimePhase> OnPhaseChanged;
    public event Action<GameDay, int> OnDayChanged;
    public event Action OnDayEnd;

    // ------------------ Unity Methods ------------------
    void Start()
    {
        LoadTime(); // Or StartNewDay() for testing
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

    // ------------------ Time Management ------------------
    public void AdvanceTime(int minutesToAdd)
    {
        currentMinute += minutesToAdd;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
        }

        // Handle overflow past "end of day"
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

    public void SkipTime(int totalMinutes) => AdvanceTime(totalMinutes);
    public void PauseTime() => IsPaused = true;
    public void ResumeTime() => IsPaused = false;
    public void TogglePause() => IsPaused = !IsPaused;

    public void StartNewDay()
    {
        currentHour = startHour;
        currentMinute = startMinute;

        DayIndex = (DayIndex + 1) % 7;
        CurrentDay = (GameDay)DayIndex;
        TotalDaysPassed++;

        OnDayChanged?.Invoke(CurrentDay, TotalDaysPassed);
        UpdatePhase();
        NotifyTimeChanged();
    }

    // ------------------ Display Helpers ------------------
    public string GetFormattedTime()
    {
        int displayHour = currentHour % 12;
        if (displayHour == 0) displayHour = 12;

        string ampm = currentHour >= 12 && currentHour < 24 ? "PM" : "AM";
        string formattedTime = $"{displayHour:D2}:{currentMinute:D2} {ampm}";
        return formattedTime;
    }

    public float GetTimeAsFloat()
    {
        return currentHour + currentMinute / 60f;
    }

    private void NotifyTimeChanged()
    {
        OnTimeChanged?.Invoke(GetFormattedTime());
    }

    // ------------------ Time Phase Management ------------------
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

        return TimePhase.Morning;
    }

    // ------------------ Save / Load ------------------
    public void SaveTime()
    {
        GameTimeData saveData = new GameTimeData
        {
            hour = currentHour,
            minute = currentMinute
        };

        PlayerPrefs.SetString("GameTime", JsonUtility.ToJson(saveData));
        PlayerPrefs.SetInt("GameDay", DayIndex);
        PlayerPrefs.SetInt("TotalDays", TotalDaysPassed);
    }

    public void LoadTime()
    {
        if (PlayerPrefs.HasKey("GameTime"))
        {
            string json = PlayerPrefs.GetString("GameTime");
            GameTimeData data = JsonUtility.FromJson<GameTimeData>(json);
            currentHour = data.hour;
            currentMinute = data.minute;

            DayIndex = PlayerPrefs.GetInt("GameDay", 0);
            CurrentDay = (GameDay)DayIndex;
            TotalDaysPassed = PlayerPrefs.GetInt("TotalDays", 0);
        }
        else
        {
            DayIndex = 6; // Forces next new day to be Monday
            StartNewDay();
        }

        UpdatePhase();
        NotifyTimeChanged();
    }

    public void ClearSavedTime()
    {
        PlayerPrefs.DeleteKey("GameTime");
        PlayerPrefs.DeleteKey("GameDay");
        PlayerPrefs.DeleteKey("TotalDays");
    }
}