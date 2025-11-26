using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseSlot : MonoBehaviour
{
    [Header("Input System")]
    PlayerInput playerInput;
    InputAction mousePositionAction;

    public InventoryItem inventoryItem;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mousePositionAction = playerInput.actions["Point"];
    }
    private void Update()
    {
        Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
        transform.position = mousePosition;
    }

    public void Clicked(InventoryItem inventoryItemInSlot)
    {
        if (inventoryItemInSlot != null)
        {
            TakeItem(inventoryItemInSlot);
        }
        else
        {
            PlaceItem();
        }
    }

    public void TakeItem(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            inventoryItem.transform.position = transform.position;
            inventoryItem.transform.SetParent(transform);
        }
        this.inventoryItem = inventoryItem;
    }

    public void PlaceItem()
    {
        this.inventoryItem = null;
    }
}
