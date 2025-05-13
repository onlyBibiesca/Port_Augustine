using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject InventoryScreen;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public static bool isInventoryClick = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    // Called via Player Input > Events > Player > Move
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Called via Player Input > Events > Player > Inventory
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            bool isActive = InventoryScreen.activeSelf;
            InventoryScreen.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0;
            isInventoryClick = !isActive;

            Debug.Log(isActive ? "Closed Inventory" : "Opened Inventory");
        }
    }
}