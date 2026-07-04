using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkSystem : MonoBehaviour, InteractableObject
{

    private GameObject player;

    private InteractableObject nearbyInteractable;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource buttonSound;

    [Header("Work")]
    [SerializeField] GameObject jobUI;
    [SerializeField] public string jobName;
    [SerializeField] int salary;
    [SerializeField] string hourlyRate;

    [Header("Wallet")]
    public Wallet wallet;
    [SerializeField] private TextMeshProUGUI walletText;

    [Header("Text")]
    public TMPro.TMP_Text salaryDisplayText;
    public TMPro.TMP_Text energyDisplayText;
    public TMPro.TMP_Text hungerDisplayText;
    public TMPro.TMP_Text jobDisplayText;

    [Header("Consumables")]
    [SerializeField] StatConsumable statConsumable;
    [SerializeField] TimeConsumable timeConsumable;

    [SerializeField] public int minimumEnergy;

    private PlayerStats playerStats;

    private void Start()
    {
        jobUI.SetActive(false);
        wallet.money = wallet.defaultValue;
    }

    private void Update()
    {
        DisplayMoney();
        UpdateDisplay();

    }

    void UpdateDisplay()
    {
        jobDisplayText.text = jobName;
        energyDisplayText.text = $"{statConsumable.energyChange}";
        hungerDisplayText.text = $"{statConsumable.hungerChange}";
        salaryDisplayText.text = $"{salary}" + "/" + $"{hourlyRate}";
    }

    public void AddMoney()
    {
        if(PlayerStats.Instance.energy >= minimumEnergy)
        {
            wallet.money = salary + wallet.money;
            Debug.Log("Salary of  " + salary + " has been added to your wallet");
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
            if (timeConsumable != null)
            {
                Debug.Log($"TimeConsumable Name: {timeConsumable.consumableName}");
                Debug.Log($"Consumes Time: {timeConsumable.consumesTime}");
                Debug.Log($"Hours: {timeConsumable.hoursToConsume}");
                Debug.Log($"Minutes: {timeConsumable.minutesToConsume}");
            }

            Debug.Log($"TimeSystem.Instance is null? {TimeSystem.Instance == null}");

            if (TimeSystem.Instance != null)
            {
                Debug.Log($"Current time BEFORE: {TimeSystem.Instance.GetFormattedTime()}");
            }

            if (timeConsumable != null && TimeSystem.Instance != null)
            {
                Debug.Log(">>> CALLING ConsumeTime() <<<");
                TimeSystem.Instance.ConsumeTime(timeConsumable);
                Debug.Log($"Current time AFTER: {TimeSystem.Instance.GetFormattedTime()}");
            }
            else if (timeConsumable == null)
            {
                Debug.LogWarning("No TimeConsumable assigned to this transition!");
            }
            else if (TimeSystem.Instance == null)
            {
                Debug.LogError("TimeSystem not found in scene!");
            }
        }

        else
        {
            Debug.Log("Not enough energy");
        }
        
    }

    public void Interact()
    {
            buttonSound.Play();
            jobUI.SetActive(true);


    }


    public void DisplayMoney()
    {
        walletText.text = $"{wallet.money}";

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

}
