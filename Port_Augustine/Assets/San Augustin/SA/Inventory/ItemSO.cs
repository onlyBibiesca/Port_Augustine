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

    [Header("Money Pricing")]
    [SerializeField] public int itemPrice;
    //leaving it blank is free, putting in value requires player's wallet

    [Header("Trait Rewards")]
    public bool grantsTraits = false;
    public List<TraitSO> grantedTraits = new();

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

        if (grantsTraits)
        {

            foreach (TraitSO trait in grantedTraits)
            {
                Debug.Log($"Granting: {trait}");
                TraitsManager.Instance.AddTrait(trait);
            }
        }

        Debug.Log($"Used {itemName}");
        PlayerStats.Instance.DebugStats();
    }
}

