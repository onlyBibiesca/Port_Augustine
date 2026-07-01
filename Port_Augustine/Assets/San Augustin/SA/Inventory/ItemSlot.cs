using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // ITEM DATA //
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public Sprite emptySprite;
    public bool isFull;

    // ITEM SLOT //
    [SerializeField]
    private Image itemImage;
    public GameObject selectedShader;
    public bool thisItemSelected;
    public string itemDescription;

    private InventoryManager inventoryManager;

    // ITEM DESCRIPTION //
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    private void Start()
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

        itemImage.sprite = itemSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();

        selectedShader.SetActive(true);
        thisItemSelected = true;

        inventoryManager.selectedSlot = this;

        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;

        if (itemSprite != null)
            itemDescriptionImage.sprite = itemSprite;
        else
            itemDescriptionImage.sprite = emptySprite;
    }

    public void EmptySlot()
    {
        itemName = "";
        quantity = 0;
        itemSprite = null;
        itemDescription = "";

        isFull = false;
        thisItemSelected = false;

        selectedShader.SetActive(false);

        itemImage.sprite = emptySprite;

        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }
}

