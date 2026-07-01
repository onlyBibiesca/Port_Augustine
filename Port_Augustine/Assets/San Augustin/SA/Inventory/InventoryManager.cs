 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;
    public ItemSlot selectedSlot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void  UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].name == itemName)
            {
                itemSOs[i].UseItem();
            }
        }

    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }
        Debug.Log("itemName = " + itemName + " quantity = " + quantity + " itemSprite = " + itemSprite);
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }

    public void UseSelectedItem()
    {
        if (selectedSlot == null)
            return;

        UseItem(selectedSlot.itemName);

        selectedSlot.EmptySlot();

        selectedSlot = null;
    }
}
