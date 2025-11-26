using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Interactive
{
    public void Interact()
    {
        PlayerInventory playerInventory = GameManager.playerInventory;
        if (playerInventory.TryAddItemToInventory(playerInventory.itemPrefab))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full");
        }
    }
}
