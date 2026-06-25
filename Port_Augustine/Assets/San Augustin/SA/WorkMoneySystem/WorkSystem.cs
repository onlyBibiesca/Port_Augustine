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
    [SerializeField] float salary;

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
