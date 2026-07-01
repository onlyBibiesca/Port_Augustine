using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkSystem : MonoBehaviour
{
    [Header("Work UI")]
    [SerializeField] GameObject workButton;
    [SerializeField] private TextMeshProUGUI walletText;

    [Header("Wallet")]
    public Wallet wallet;

    [Header("Work")]
    [SerializeField] int salary;

    [Header("Consumables")]
    [SerializeField] StatConsumable statConsumable;
    [SerializeField] TimeConsumable timeConsumable;

    private void Start()
    {
        workButton.SetActive(false);
        wallet.money = wallet.defaultValue;
    }

    private void Update()
    {
        DisplayMoney();
    }

    public void AddMoney()
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

    public void DisplayMoney()
    {
        walletText.text = $"{wallet.money}";

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            workButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            workButton.SetActive(false);
        }
    }

}
