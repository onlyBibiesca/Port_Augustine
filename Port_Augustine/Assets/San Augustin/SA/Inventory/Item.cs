using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private ItemSO itemSO;

    [SerializeField]
    private Wallet wallet;
    //player wallet

    [SerializeField]
    private Sprite sprite;

    [TextArea]
    [SerializeField]
    private string itemDescription;

    private InventoryManager inventoryManager;

    [SerializeField] private bool canOnlyBeCollectedOnce = false;
    private bool hasBeenCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
   
    }

    public void PickUpItem()
    {
        if(wallet.money >= itemSO.itemPrice)
        {
            if (canOnlyBeCollectedOnce && hasBeenCollected)
                return;

            InventoryManager.Instance.AddItem(itemName, quantity, sprite, itemDescription);

            if (canOnlyBeCollectedOnce)
            {
                hasBeenCollected = true;

                // Gray out the button
                GetComponent<UnityEngine.UI.Image>().color = Color.gray;

                // Prevent further clicks
                GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }
        else
        {
            Debug.Log("Not Enough money");
        }
    }
}
