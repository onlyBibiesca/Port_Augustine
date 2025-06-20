using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;





public class ItemDatabase : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;

    //Item Slot
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    //Item Description
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;
   
       void Start()
       {
           inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
       }
   



    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if (itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }
        Debug.Log("Clicked on item: " + itemName);
    }

    public void OnRightClick()
    {

    }
    /*
    public void UseButton()
    {
        Debug.Log("UseButton pressed for: " + itemName);
        if (thisItemSelected == true)
        {
            inventoryManager.UseItem(itemName);
            EmptySlot();
        }
    }
    */

    public void UseButton()
    {
        // Ensure only the selected item is used
        foreach (var item in inventoryManager.itemData)
        {
            if (item.thisItemSelected)
            {
                Debug.Log("Using item: " + item.itemName);
                inventoryManager.UseItem(item.itemName);
                item.EmptySlot(); // Remove item after use
                return; // Exit after using the selected item
            }
        }

        Debug.LogWarning("No item selected!");
    }
    public void EmptySlot()
    {
        thisItemSelected = true;
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        itemDescription = "";
        itemName = "";
        itemSprite = emptySprite;
        isFull = false;

        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }
}