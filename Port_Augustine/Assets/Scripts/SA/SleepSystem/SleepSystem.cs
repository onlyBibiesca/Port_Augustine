using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSystem : MonoBehaviour, InteractableObject
{

    private GameObject player;
    private InteractableObject nearbyInteractable;
    public static SleepSystem Instance;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;

    [Header("Sleep Settings")]
    public int defaultWakeUpHour = 8;
    public int defaultWakeUpMinute = 0;

    [Range(1, 16)]
    public int defaultSleepDuration = 8; // How many hours is "full rest"

    [Header("Energy Recovery")]
    [Range(0, 100)]
    public int maxEnergyRecovery = 100; // Max energy gained from sleep

    [Range(0, 100)]
    public int minEnergyRecovery = 10; // Min energy gained from sleep (even short naps help)

    public bool allowOversleep = true; // Can you sleep more than default for extra energy?
    [Range(0, 50)]
    public int oversleepBonus = 10; // Extra energy per hour over default

    [Header("Debug")]
    public bool showDebugMessages = true;

    private int sleepStartHour = -1;
    private int sleepStartMinute = -1;
    private bool isSleeping = false;

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

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    public void Interact()
    {
        Debug.Log("========== INTERACT CALLED ==========");
        Debug.Log($"Player is null? {player == null}");

        SleepSystem.Instance.GoToSleep();
        SleepSystem.Instance.WakeUpAtDefaultTime();
    }

    // Player goes to sleep at current time
    public void GoToSleep()
    {
        if (TimeSystem.Instance == null)
        {
            Debug.LogError("TimeSystem not found!");
            return;
        }

        sleepStartHour = TimeSystem.Instance.currentHour;
        sleepStartMinute = TimeSystem.Instance.currentMinute;
        isSleeping = true;

        if (showDebugMessages)
            Debug.Log($"Player went to sleep at {TimeSystem.Instance.GetFormattedTime()}");
    }

    // Player wakes up at default time
    public void WakeUpAtDefaultTime()
    {
        if (!isSleeping)
        {
            Debug.LogWarning("Player is not sleeping!");
            return;
        }

        WakeUp(defaultWakeUpHour, defaultWakeUpMinute);
    }

    // Player wakes up at custom time
    public void WakeUp(int wakeUpHour, int wakeUpMinute)
    {
        if (!isSleeping)
        {
            Debug.LogWarning("Player is not sleeping!");
            return;
        }

        if (TimeSystem.Instance == null || PlayerStats.Instance == null)
        {
            Debug.LogError("TimeSystem or PlayerStats not found!");
            return;
        }

        // Calculate sleep duration
        int sleepDurationHours = 0;
        int sleepDurationMinutes = 0;

        // Account for sleeping across midnight
        if (wakeUpHour >= sleepStartHour)
        {
            // Same day sleep (shouldn't happen in normal use)
            sleepDurationHours = wakeUpHour - sleepStartHour;
            sleepDurationMinutes = wakeUpMinute - sleepStartMinute;
        }
        else
        {
            // Overnight sleep
            sleepDurationHours = (24 - sleepStartHour) + wakeUpHour;
            sleepDurationMinutes = wakeUpMinute - sleepStartMinute;
        }

        // Handle negative minutes
        if (sleepDurationMinutes < 0)
        {
            sleepDurationHours--;
            sleepDurationMinutes += 60;
        }

        if (showDebugMessages)
            Debug.Log($"Player slept for {sleepDurationHours}h {sleepDurationMinutes}m");

        // Calculate energy recovery
        int energyRecovered = CalculateEnergyRecovery(sleepDurationHours);

        if (showDebugMessages)
            Debug.Log($"Energy recovered: {energyRecovered}");

        // Apply energy recovery
        PlayerStats.Instance.ChangeEnergy(energyRecovered);

        // Advance time to wake up time
        TimeSystem.Instance.SetTime(wakeUpHour, wakeUpMinute);

        // Reset sleep state
        isSleeping = false;
        sleepStartHour = -1;
        sleepStartMinute = -1;

        if (showDebugMessages)
            Debug.Log($"Player woke up at {TimeSystem.Instance.GetFormattedTime()}");
    }

    // Calculate energy recovery based on sleep duration
    int CalculateEnergyRecovery(int hoursSlept)
    {
        int energyRecovered = 0;

        if (hoursSlept <= 0)
        {
            energyRecovered = minEnergyRecovery;
        }
        else if (hoursSlept >= defaultSleepDuration)
        {
            // Full or more than default sleep
            energyRecovered = maxEnergyRecovery;

            // Bonus energy if sleeping more than default
            if (allowOversleep && hoursSlept > defaultSleepDuration)
            {
                int extraHours = hoursSlept - defaultSleepDuration;
                int bonus = extraHours * oversleepBonus;
                energyRecovered += bonus;
                energyRecovered = Mathf.Min(energyRecovered, 100); // Cap at 100
            }
        }
        else
        {
            // Partial sleep - proportional recovery
            float recoveryRatio = (float)hoursSlept / defaultSleepDuration;
            energyRecovered = Mathf.RoundToInt(maxEnergyRecovery * recoveryRatio);
            energyRecovered = Mathf.Max(energyRecovered, minEnergyRecovery); // At least min
        }

        return energyRecovered;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            InteractableObject interactable = collision.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                nearbyInteractable = interactable;
                if (interactUI != null)
                    interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractableObject>() == nearbyInteractable)
        {
            nearbyInteractable = null;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    public bool IsSleeping()
    {
        return isSleeping;
    }

    public string GetSleepInfo()
    {
        if (!isSleeping)
            return "Player is not sleeping";

        return $"Sleeping since {sleepStartHour:00}:{sleepStartMinute:00}. Default wake: {defaultWakeUpHour:00}:{defaultWakeUpMinute:00}";
    }
}
