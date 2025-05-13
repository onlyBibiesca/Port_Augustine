using UnityEngine;
using TMPro;

public class TimeControl : MonoBehaviour
{
    public TimeManager timeSystem;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI phaseText;

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

        // Set initial values
        UpdateTime(timeSystem.GetFormattedTime());
        UpdatePhase(timeSystem.CurrentPhase);
    }

    private void OnDestroy()
    {
        if (timeSystem != null)
        {
            timeSystem.OnTimeChanged -= UpdateTime;
            timeSystem.OnPhaseChanged -= UpdatePhase;
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
}