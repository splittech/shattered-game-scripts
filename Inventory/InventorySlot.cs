using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem inventoryItem;

    public void Clicked(InventoryItem inventoryItemOnMouse)
    {
        if (inventoryItemOnMouse != null)
        {
            PlaceItemInSlot(inventoryItemOnMouse);
        }
        else
        {
            TakeItemFromSlot();
        }
    }

    public void PlaceItemInSlot(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            inventoryItem.transform.position = transform.position;
            inventoryItem.transform.SetParent(transform);
        }
        this.inventoryItem = inventoryItem;
    }

    public void TakeItemFromSlot()
    {
        this.inventoryItem = null;
    }
}
