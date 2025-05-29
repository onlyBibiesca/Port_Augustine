using UnityEngine;
using TMPro;

public class TimeControl : MonoBehaviour
{
    public TimeManager timeSystem;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI dayText; //  New: To display the day

    private void Start()
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("TimeSystem is not assigned!");
            return;
        }

        // Subscribe to updates
        timeSystem.OnTimeChanged += UpdateTime;
        timeSystem.OnPhaseChanged += UpdatePhase;
        timeSystem.OnDayChanged += UpdateDay; // Subscribe to day change

        // Set initial values
        UpdateTime(timeSystem.GetFormattedTime());
        UpdatePhase(timeSystem.CurrentPhase);
        UpdateDay(timeSystem.CurrentDay, timeSystem.TotalDaysPassed); //  Initial day text
    }

    private void OnDestroy()
    {
        if (timeSystem != null)
        {
            timeSystem.OnTimeChanged -= UpdateTime;
            timeSystem.OnPhaseChanged -= UpdatePhase;
            timeSystem.OnDayChanged -= UpdateDay; //  Unsubscribe to avoid memory leaks
        }
    }

    private void UpdateTime(string time)
    {
        if (timeText != null)
            timeText.text = time;
    }

    private void UpdatePhase(TimePhase phase)
    {
        if (phaseText != null)
            phaseText.text = phase.ToString();
    }

    private void UpdateDay(GameDay day, int totalDays)
    {
        if (dayText != null)
            //dayText.text = day.ToString();
            dayText.text = day.ToString() + " Day " + totalDays.ToString();
        // dayText.text = $"Day: {day} ({totalDays} total)";
    }
}