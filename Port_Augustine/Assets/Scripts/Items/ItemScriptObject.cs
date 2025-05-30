using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;
using System.ComponentModel.Design;

[CreateAssetMenu]
public class ItemScriptObject : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    //public List<StatToChange> statchange;
    public List<StatChange> statChanges = new List<StatChange>();
    //public StatToChange statToChange = new StatToChange();
    //public int amountChangeStat;
    
    /*
    [Header("PlayerStats")]
    public PlayerStats playerStats;
    */

    public void UseItem()
    {

        //PlayerManager playerManager = FindObjectOfType<PlayerManager>();



        foreach (StatChange change in statChanges)
        {

            switch (change.stat)
            {
                case StatToChange.health:
                    //playerManager.ChangeHealth(change.amount);
                    Debug.Log("Health Added: " + change.amount);
                    break;

                case StatToChange.hunger:
                   // playerManager.ChangeHunger(change.amount);
                    Debug.Log("Health Added: " + change.amount);
                    break;

                case StatToChange.energy:
                   // playerManager.ChangeEnergy(change.amount);
                    Debug.Log("Health Added: " + change.amount);
                    break;

                case StatToChange.socialbatt:
                    //playerManager.ChangeSocialBattery(change.amount);
                    Debug.Log("Health Added: " + change.amount);
                    break;

            }
        }
            
        
    }


    [System.Serializable]
    public class StatChange
    {
        public StatToChange stat;
        public int amount;
    }
    //sets the item stats
    public enum StatToChange
    {
        none,
        health,
        hunger,
        energy,
        socialbatt,

    };


}
