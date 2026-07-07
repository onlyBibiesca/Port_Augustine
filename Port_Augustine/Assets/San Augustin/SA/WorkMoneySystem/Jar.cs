using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Jar : MonoBehaviour
{
    public Wallet wallet;
    private int savingJar;

    [Header("Text")]
    public TMPro.TMP_Text savingsDisplayText;

    [Header("Input Money")]
    public int inSave = 100;
    public int outSave = 100;

    public void Update()
    {
        DisplaySavings();
    }

    public void PutSavings()
    {
        if (wallet != null && wallet.money >= inSave)
        {
            savingJar += inSave;
            wallet.money -= inSave;
        }
        else if (wallet == null)
        {
            Debug.LogError("WHERE THE FUCK IST HE WALLET???????");
        }
        else
        {
            Debug.LogError("WE ARE BROKE THERE'S NOTHING TO SAVE UP FOR");
        }
        
    }

    public void OutSavings()
    {

        if (wallet != null && wallet.money >= inSave)
        {
            savingJar -= outSave;
            wallet.money += outSave;
        }
        else if (wallet == null)
        {
            Debug.LogError("WHERE THE FUCK IST HE WALLET???????");
        }
        else
        {
            Debug.LogError("WE ARE BROKE THERE'S NOTHING TO GET FROM");
        }
    }

    void DisplaySavings()
    {
        savingsDisplayText.text = $"{savingJar}";
    }

}
