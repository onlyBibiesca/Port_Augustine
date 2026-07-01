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

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
   
    }

    public void PickUpItem()
    {
       
        if(wallet.money >= itemSO.itemPrice)
        {
            wallet.money -= itemSO.itemPrice;
            InventoryManager.Instance.AddItem(itemName, quantity, sprite, itemDescription);
        }
        else
        {
            Debug.Log("Insufficient Funds!");
        }
        

        
    }
}
