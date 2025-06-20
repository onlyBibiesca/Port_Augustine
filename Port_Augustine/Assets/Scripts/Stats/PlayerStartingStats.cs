using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats")]
public class PlayerStartingStats : ScriptableObject
{
    public string traitID;

    public int health;
    public int hunger;
    public int energy;
    public int socialBattery;
    public int money;
}
