using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    private QuestInventoryController questInventoryController;

    // Start is called before the first frame update
    void Start()
    {
        questInventoryController = FindObjectOfType<QuestInventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item> ();
            if (item != null)
            {
                bool itemAdded = questInventoryController.AddItem(collision.gameObject);

                if(itemAdded)
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
