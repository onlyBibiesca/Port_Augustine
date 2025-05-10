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
            /*
           if (statToChange == StatToChange.health)
           {
               //example codes if stats are scripted
               //GameObject.Find("HealthManager").GetComponent<Health>().ChangeHealth(amountChangeStat);
               //healthSlider.value += playerStats.hunger;
               playerManager.ChangeHealth(amountChangeStat);
               Debug.Log("Health Added");
           }

           if (statToChange == StatToChange.hunger)
           {
               playerManager.ChangeHunger(amountChangeStat);
               Debug.Log("Hunger Added");
           }

           if (statToChange == StatToChange.energy)
           {
               //GameObject.Find("StaminaManager").GetComponent<Stamina>().ChangeStamina(amountChangeStat);
               playerManager.ChangeEnergy(amountChangeStat);
               Debug.Log("Stamina Added");
           }

           if (statToChange == StatToChange.socialbatt)
           {
               //GameObject.Find("StaminaManager").GetComponent<Stamina>().ChangeStamina(amountChangeStat);
               playerManager.ChangeSocialBattery(amountChangeStat);
               Debug.Log("Social Battery Added");
           }
        
            */
        
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
