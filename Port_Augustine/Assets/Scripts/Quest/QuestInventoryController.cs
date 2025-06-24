using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class QuestInventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCOunt;
    public GameObject[] itemPrefabs;

    public static QuestInventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        itemDictionary = FindObjectOfType<ItemDictionary>();
        for (int i = 0; i < slotCOunt; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
                
            }
        }
    }

    /*public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();  
                if(item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + Item.quantity;
                }
                
            }
        }
    }*/
    public bool AddItem(GameObject itemPrefab)
    {
        //look for empty slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>(); 
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Quest Inventory is full");
        return false;
    }
}
