using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayFilter : MonoBehaviour
{
    public static TimeOfDayFilter Instance;

    [Header("Morning Settings (6:00 - 11:59)")]
    public Color morningColor = new Color(1f, 0.95f, 0.85f, 0.15f); // Warm yellow tint
    public float morningIntensity = 0.15f;

    [Header("Afternoon Settings (12:00 - 16:59)")]
    public Color afternoonColor = new Color(1f, 1f, 1f, 0f); // Neutral/no tint
    public float afternoonIntensity = 0f;

    [Header("Evening Settings (17:00 - 18:59)")]
    public Color eveningColor = new Color(1f, 0.7f, 0.5f, 0.2f); // Orange/golden tint
    public float eveningIntensity = 0.2f;

    [Header("Night Settings (19:00 - 5:59)")]
    public Color nightColor = new Color(0.3f, 0.4f, 0.6f, 0.4f); // Blue tint
    public float nightIntensity = 0.4f;

    [Header("UI References")]
    public Image filterImage; // Full screen overlay image
    public Text timeOfDayLabel; // Optional - shows current time period

    [Header("Smooth Transition")]
    public bool useSmoothing = true;
    public float transitionSpeed = 0.5f; // How fast to transition between colors

    private Color currentFilterColor;
    private Color targetFilterColor;

    enum TimeOfDay { Morning, Afternoon, Evening, Night }

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
        // Create filter if not assigned
        if (filterImage == null)
        {
            CreateFilterUI();
        }

        currentFilterColor = afternoonColor;
        if (filterImage != null)
            filterImage.color = currentFilterColor;

        // Subscribe to time changes
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeChanged += UpdateFilter;
            // Initial update
            UpdateFilter(TimeSystem.Instance.currentHour, TimeSystem.Instance.currentMinute);
        }
    }

    void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeChanged -= UpdateFilter;
        }
    }

    void Update()
    {
        // Smoothly transition to target color
        if (useSmoothing && filterImage != null)
        {
            currentFilterColor = Color.Lerp(currentFilterColor, targetFilterColor, Time.deltaTime * transitionSpeed);
            filterImage.color = currentFilterColor;
        }
    }

    void UpdateFilter(int hour, int minute)
    {
        TimeOfDay timeOfDay = GetTimeOfDay(hour);
        targetFilterColor = GetColorForTimeOfDay(timeOfDay);

        if (!useSmoothing && filterImage != null)
        {
            filterImage.color = targetFilterColor;
        }

        UpdateTimeLabel(timeOfDay);

        Debug.Log($"Time filter updated: {timeOfDay} ({hour:00}:{minute:00})");
    }

    TimeOfDay GetTimeOfDay(int hour)
    {
        if (hour >= 6 && hour < 12)
            return TimeOfDay.Morning;
        else if (hour >= 12 && hour < 17)
            return TimeOfDay.Afternoon;
        else if (hour >= 17 && hour < 18)
            return TimeOfDay.Evening;
        else
            return TimeOfDay.Night; // 21:00 - 05:59
    }

    Color GetColorForTimeOfDay(TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Morning:
                return morningColor;
            case TimeOfDay.Afternoon:
                return afternoonColor;
            case TimeOfDay.Evening:
                return eveningColor;
            case TimeOfDay.Night:
                return nightColor;
            default:
                return afternoonColor;
        }
    }

    void UpdateTimeLabel(TimeOfDay timeOfDay)
    {
        if (timeOfDayLabel != null)
        {
            timeOfDayLabel.text = timeOfDay.ToString();
        }
    }

    // Create filter UI if not assigned
    void CreateFilterUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FilterCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Create filter image
        GameObject filterObj = new GameObject("TimeOfDayFilter");
        filterObj.transform.SetParent(canvas.transform, false);

        filterImage = filterObj.AddComponent<Image>();
        filterImage.color = afternoonColor;

        // Make it full screen
        RectTransform rect = filterObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Set to back (low sorting order)
        canvas.sortingOrder = -1;

        Debug.Log("TimeOfDayFilter UI created automatically");
    }

    // Get current time of day
    public string GetCurrentTimeOfDayName()
    {
        return GetTimeOfDay(TimeSystem.Instance.currentHour).ToString();
    }

    // Manually set filter color
    public void SetFilterColor(Color newColor)
    {
        targetFilterColor = newColor;
        if (!useSmoothing && filterImage != null)
        {
            filterImage.color = newColor;
        }
    }
}
