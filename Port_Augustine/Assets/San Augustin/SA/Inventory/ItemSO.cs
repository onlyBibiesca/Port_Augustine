using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;

    [Header("Stat Modifiers")]
    public int ChangeHunger = 0;
    public int ChangeEnergy = 0;
    public int ChangeHappiness = 0;

    public void UseItem()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats Instance not found!");
            return;
        }

        PlayerStats.Instance.ChangeHunger(ChangeHunger);
        PlayerStats.Instance.ChangeEnergy(ChangeEnergy);
        PlayerStats.Instance.ChangeHappiness(ChangeHappiness);

        Debug.Log($"Used {itemName}");
        PlayerStats.Instance.DebugStats();
    }
}


