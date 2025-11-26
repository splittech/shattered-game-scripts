using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject itemPrefab;

    public InventorySlot initialInventorySlot;

    public MouseSlot mouseSlot;

    void Start()
    {
        GameObject item = Instantiate(itemPrefab);
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
        initialInventorySlot.PlaceItemInSlot(inventoryItem);
    }

    public void Clicked(InventorySlot inventorySlot)
    {
        InventoryItem mouseInventoryItem = mouseSlot.inventoryItem;
        InventoryItem slotInventoryItem = inventorySlot.inventoryItem;

        inventorySlot.PlaceItemInSlot(mouseInventoryItem);
        mouseSlot.TakeItem(slotInventoryItem);
    }
}
