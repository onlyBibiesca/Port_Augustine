using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Wallet", menuName = "PlayerWallet")]
public class Wallet : ScriptableObject
{
    public float money;
    public float defaultValue;

    public void PrintMessage()
    {
        Debug.Log("Wallet has been loaded");
    }
}
