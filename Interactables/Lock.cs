using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Lock : MonoBehaviour, ISignalGenerator
{
    public event Action<bool> OnSignalChanged;

    [Header("References")]
    Interactive interactive;
    PlayerInventory playerInventory;

    private void Awake()
    {
        interactive = GetComponent<Interactive>();

        interactive.OnInteracted += Interact;
    }

    private void Start()
    {
        playerInventory = GameManager.playerInventory;
    }

    private void Interact()
    {
        bool usedKey = playerInventory.RemoveItemFromInventory();
        if (usedKey)
        {
            OnSignalChanged?.Invoke(true);
            Destroy(gameObject);
        }
        else
        {
            DebugHelper.LogWithObject(gameObject, "No Key", "");
        }
    }

    public bool GetCurrentSignal()
    {
        return false;
    }
}
