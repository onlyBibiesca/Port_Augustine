using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Wallet", menuName = "PlayerWallet")]
public class Wallet : ScriptableObject
{
    public int money;
    public int defaultValue;

    public void AddMoney(int amount)
    {
        money += amount;

        DailySummaryManager.Instance?.RecordMoneyEarned(amount);
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        return true;
    }

    public void PrintMessage()
    {
        Debug.Log("Wallet has been loaded");
    }
}