using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Key : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory inventory;
    public Interactive interactive;

    private void Awake()
    {
        interactive = GetComponent<Interactive>();

        interactive.OnInteracted += Interacted;
    }

    private void Start()
    {
        inventory = GameManager.playerInventory;
    }

    void Interacted()
    {
        bool keyAdded = inventory.AddItemToInventory(inventory.itemPrefab);
        if (keyAdded)
        {
            Destroy(gameObject);
        }
        else
        {
            DebugHelper.LogWithObject(gameObject, "Inventory Is full", "");
        }
    }
}
