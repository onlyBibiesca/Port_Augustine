using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SleepSystem : MonoBehaviour, InteractableObject
{

    private GameObject player;
    private InteractableObject nearbyInteractable;
    public static SleepSystem Instance;

    private PlayerInput playerInput;
    private bool playerMovementEnabled = true;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource buttonSound;



    [Header("Room")]
    [SerializeField] GameObject PlayerUI;
    [SerializeField] GameObject homeUI;

    [Header("Sleep Settings")]
    public int defaultWakeUpHour = 8;
    public int defaultWakeUpMinute = 0;

    [Range(1, 16)]
    public int defaultSleepDuration = 8; // How many hours is "full rest"

    [Header("Passed Out UI")]
    [SerializeField] GameObject passedOutUI;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float showTime = 3f;
    private Coroutine currentRoutine;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Energy Recovery")]
    [Range(0, 100)]
    public int maxEnergyRecovery = 100; // Max energy gained from sleep

    [Range(0, 100)]
    public int minEnergyRecovery = 10; // Min energy gained from sleep (even short naps help)

    public bool allowOversleep = true; // Can you sleep more than default for extra energy?
    [Range(0, 50)]
    public int oversleepBonus = 10; // Extra energy per hour over default

    [Header("Stat Consumable")]
    [SerializeField] StatConsumable statConsumable;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private int sleepStartHour = -1;
    private int sleepStartMinute = -1;
    private bool isSleeping = false;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

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

    private void Update()
    {

    }

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    public void ShowTemporarily()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndHide());
    }

    private IEnumerator ShowAndHide()
    {
        passedOutUI.SetActive(true);
        // Start transparent
        canvasGroup.alpha = 0f;
        //S Fade in
        yield return StartCoroutine(Fade(0f, 1f));

        // Stay visible
        yield return new WaitForSeconds(showTime);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(showTime);
        passedOutUI.SetActive(false);
        currentRoutine = null;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }


    public void Interact()
    {
        buttonSound.Play();
        DisablePlayerMovement();
        homeUI.SetActive(true);
        PlayerUI.SetActive(false);
        

    }
    public void SleepButton()
    {
        Debug.Log("========== Sleep Button CALLED ==========");
        Debug.Log($"Player is null? {player == null}");

        SleepSystem.Instance.GoToSleep();
        SleepSystem.Instance.WakeUpAtDefaultTime();

        if (PlayerStats.Instance != null && statConsumable != null)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.ConsumeStat(statConsumable);
            }
            else
            {
                Debug.LogError("PlayerStats not found in scene!");
            }
        }
    }
    public void MidnightForceSleep()
    {
        SleepSystem.Instance.GoToSleep();
        SleepSystem.Instance.WakeUpAtDefaultTime();

        ShowTemporarily();

        DailySummaryUI.Instance.ShowSummary();


    }

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

        int wakeHour = defaultWakeUpHour;

        if (TraitsManager.Instance != null)
        {
            wakeHour += TraitsManager.Instance.GetWakeUpHourModifier();
        }

        wakeHour = Mathf.Clamp(wakeHour, 0, 23);

        if (showDebugMessages)
        {
            Debug.Log($"Wake-up modifier applied. Waking at {wakeHour:00}:{defaultWakeUpMinute:00}");
        }

        WakeUp(wakeHour, defaultWakeUpMinute);
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
        int daysSlept = 0;

        // Account for sleeping across midnight
        if (wakeUpHour > sleepStartHour)
        {
            // Same day sleep (shouldn't happen in normal use)
            sleepDurationHours = wakeUpHour - sleepStartHour;
            sleepDurationMinutes = wakeUpMinute - sleepStartMinute;
            daysSlept = 0;
        }
        else
        {
            // Overnight sleep - woke up on next day
            sleepDurationHours = (24 - sleepStartHour) + wakeUpHour;
            sleepDurationMinutes = wakeUpMinute - sleepStartMinute;
            daysSlept = 1;
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

        if (daysSlept > 0)
        {
            TimeSystem.Instance.SetDay(TimeSystem.Instance.currentDay + daysSlept);
            if (showDebugMessages)
            {
                Debug.Log($"Advanced to Day {TimeSystem.Instance.currentDay}");
            }

        }

        DailySummaryUI.Instance.ShowSummary();



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

    void FindPlayerInput()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogWarning("PlayerInput component not found!");
            }
        }
    }

    public void DisablePlayerMovement()
    {
        FindPlayerInput();
        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerMovementEnabled = false;
            Debug.Log("Player movement disabled");
        }
    }

    public void EnablePlayerMovement()
    {
        FindPlayerInput();
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerMovementEnabled = true;
            Debug.Log("Player movement enabled");
        }
    }


}
