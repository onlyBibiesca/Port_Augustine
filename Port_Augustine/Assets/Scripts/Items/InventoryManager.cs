using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{


    public GameObject InventoryScreen;
    public static bool isInventoryClick = false;
    public ItemDatabase[] itemData;
    //private PlayerControls playerControls;

    public ItemScriptObject[] ItemSO;

    private void Awake()
    {
        //playerControls = new PlayerControls();
        //playerControls.UI.Inventory.performed += ToggleInventory;
    }

    void Start()
    {
        foreach (var item in ItemSO)
        {
            Debug.Log("ItemSO contains: " + item.itemName);
        }
    }


    /*
    public void OnInventoryBag()
    {
        Time.timeScale = 0;
        InventoryScreen.SetActive(true);
        isInventoryClick = true;
        Debug.Log("Going to Inventory");
    }

    public void ExitInventoryBag()
    {
        Time.timeScale = 1;
        InventoryScreen.SetActive(false);
        isInventoryClick = false;
        Debug.Log("Exit Inventory");
    }

    */

    private void OnEnable()
    {
        //playerControls.Enable();
    }

    private void OnDisable()
    {
        //playerControls.Disable();
    }

    private void ToggleInventory(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!InventoryScreen.activeSelf)
        {
            Time.timeScale = 0;
            InventoryScreen.SetActive(true);
            isInventoryClick = true;
            Debug.Log("Opened Inventory");
        }
        else
        {
            Time.timeScale = 1;
            InventoryScreen.SetActive(false);
            isInventoryClick = false;
            Debug.Log("Closed Inventory");
        }
    }

    public void UseItem(string itemName)
    {
        Debug.Log("Searching for item to use: " + itemName);
        for (int i = 0; i < ItemSO.Length; i++)
        {
            Debug.Log("Checking item: " + ItemSO[i].itemName);
            if (ItemSO[i].itemName == itemName)
            {
                Debug.Log("Using item: " + itemName);
                ItemSO[i].UseItem();
                break; // Stops after finding the first match
            }
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemData.Length; i++)
        {
            if (itemData[i].isFull == false)
            {
                itemData[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                Debug.Log("Item Received" + itemName + " " + quantity + " " + itemSprite);
                return;
            }
        }

    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemData.Length; i++)
        {
            itemData[i].selectedShader.SetActive(false);
            itemData[i].thisItemSelected = false;
        }
    }
}
