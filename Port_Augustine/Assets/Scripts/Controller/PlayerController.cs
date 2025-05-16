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
}