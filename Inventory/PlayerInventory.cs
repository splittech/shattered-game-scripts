using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInventory : MonoBehaviour
{
    public GameObject itemPrefab;

    public InventorySlot initialInventorySlot;

    public InventorySlot[] inventorySlots;

    public MouseSlot mouseSlot;

    public GameObject wholeInventory;

    [Header("Input System")]
    PlayerInput playerInput;
    InputAction inventoryButtonAction;

    private void Awake()
    {
        GameManager.playerInventory = this;

        playerInput = GetComponent<PlayerInput>();

        inventoryButtonAction = playerInput.actions["Inventory"];

        inventoryButtonAction.started += ToggleInventory;
    }

    void ToggleInventory(InputAction.CallbackContext context)
    {
        wholeInventory.SetActive(!wholeInventory.activeSelf);
    }

    public void Clicked(InventorySlot inventorySlot)
    {
        InventoryItem mouseInventoryItem = mouseSlot.inventoryItem;
        InventoryItem slotInventoryItem = inventorySlot.inventoryItem;

        inventorySlot.PlaceItemInSlot(mouseInventoryItem);
        mouseSlot.TakeItem(slotInventoryItem);
    }

    public bool AddItemToInventory(GameObject itemPrefab)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].inventoryItem == null)
            {
                GameObject item = Instantiate(itemPrefab);
                InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
                inventorySlots[i].PlaceItemInSlot(inventoryItem);
                return true;
            }
        }
        
        return false;
    }

    public bool RemoveItemFromInventory()
    {
        for (int i = inventorySlots.Length - 1; i >= 0; i--)
        {
            if (inventorySlots[i].inventoryItem != null)
            {
                inventorySlots[i].TakeItemFromSlot();
                return true;
            }
        }
        
        return false;
    }
}
