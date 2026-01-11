using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 5f;
    public float maxDropDistance = 8f;

    private QuestInventoryController inventoryController;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = QuestInventoryController.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; 
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();

        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        // slot under drop point
        if (dropSlot != null)
        {
            if(dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if(draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }

                else
                {
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //center
                }

             
            }

            else
            {
                originalSlot.currentItem = null;
                //move item into dropslot
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //center
            }

            /*//move item into dropslot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;*/
        }

        else
        {
            //no slot under drop point
            transform.SetParent(originalParent);

            //if dropping not wtihing inventory
            if (!isWithinInventory(eventData.position))
            {
                //drop item
                DropItem(originalSlot);
            }
            else
            {
                //snap back to recent spot\
                transform.SetParent(originalParent);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //center
            }
        }
        
    }

    bool isWithinInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        int quantity = item.quantity;

        if(quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            quantity = 1;
        }
        else
        {
            originalSlot.currentItem = null;
        }

        

        //find the player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if(playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }
        // random drop near player
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        //insantiate drop item
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);
        Item droppedItem = dropItem.GetComponent<Item>();
        droppedItem.quantity = 1;

        //destroy ui
        if(quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);
        }

        QuestInventoryController.Instance.RebuildItemCounts();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            //split stack
            SplitStack();
        }
    }

    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);

        GameObject newItem = item.CloneItem(splitAmount);

        if(inventoryController == null || newItem == null) return;

        foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }

        //no empty slot return to stack
        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}
