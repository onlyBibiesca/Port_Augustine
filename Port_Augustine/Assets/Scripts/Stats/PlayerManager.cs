using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStartingStats stats;

    [Header("Sliders")]
    public Slider healthSlider;
    public Slider energySlider;
    public Slider hungerSlider;
    public Slider socialbatSlider;

    private void Start()
    {
        SetStats();
    }

    public void SetStats()
    {
        healthSlider.value = stats.health;
        hungerSlider.value = stats.hunger;
        energySlider.value = stats.energy;
        socialbatSlider.value = stats.socialBattery;
    }

    public void ChangeHealth(int amount)
    {
        stats.health = Mathf.Clamp(stats.health + amount, 0, (int)healthSlider.maxValue);
        healthSlider.value = (float)stats.health;
    }

    public void ChangeHunger(int amount)
    {
        stats.hunger = Mathf.Clamp(stats.hunger + amount, 0, (int)hungerSlider.maxValue);
        hungerSlider.value = (float)stats.hunger;
    }

    public void ChangeEnergy(int amount)
    {
        stats.energy = Mathf.Clamp(stats.energy + amount, 0, (int)energySlider.maxValue);
        energySlider.value = (float)stats.energy;
    }

    public void ChangeSocialBattery(int amount)
    {
        stats.socialBattery = Mathf.Clamp(stats.socialBattery + amount, 0, (int)socialbatSlider.maxValue);
        socialbatSlider.value = (float)stats.socialBattery;
    }
}
