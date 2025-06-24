using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour, InteractableObject
{
    [Header("UI Elements")]
    public GameObject Menu;
    public GameObject playerUI;

    [Header("Shop NPC")]
    public GameObject npc;

    public static bool isActive = false;

    private void Start()
    {
        isActive = false;
        Menu.SetActive(false);
    }

    public void Interact()
    {
        if (!isActive)
        {
            MenuFood();
        }
    }

    public void MenuFood()
    {
        isActive = true;
        Menu.SetActive(true);
        playerUI.SetActive(false);
        npc.SetActive(false);
        Time.timeScale = 0;
    }

    public void Exit()
    {
        isActive = false;
        Menu.SetActive(false);
        playerUI.SetActive(true);
        npc.SetActive(true);
        Time.timeScale = 1;
    }
}
