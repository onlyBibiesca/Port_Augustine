using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Wallet", menuName = "PlayerWallet")]
public class Wallet : ScriptableObject
{
    public int money = 0;
    public int defaultValue = 0;

    public void AddMoney(int amount)
    {
        money = amount + money;

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