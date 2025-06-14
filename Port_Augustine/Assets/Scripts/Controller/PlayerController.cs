using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject InventoryScreen;
    [SerializeField] private GameObject debugScreen;
    [SerializeField] private GameObject questScreen;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public static bool isInventoryClick, isDebugClick, isQuestClick = false;

    [HideInInspector] public float moveSpeedModifier = 0f; // Trait system will modify this

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = moveInput * (moveSpeed + moveSpeedModifier);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = InventoryScreen.activeSelf;
            InventoryScreen.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0;
            isInventoryClick = !isActive;

            Debug.Log(isActive ? "Closed Inventory" : "Opened Inventory");
        }
    }

    public void OnDebug(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = debugScreen.activeSelf;
            debugScreen.SetActive(!isActive);
            isDebugClick = !isActive;

            Debug.Log(isActive ? "Closed Debug Screen" : "Opened Debug Screen");
        }
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = questScreen.activeSelf;
            questScreen.SetActive(!isActive);
            isQuestClick = !isActive;

            Debug.Log(isActive ? "Closed Quest Screen" : "Opened Quest Screen");
        }
    }
}