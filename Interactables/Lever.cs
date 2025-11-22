using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject, ISignalGenerator
{
    public event Action<bool> OnSignalChanged;
    bool isActive = false;
    protected override void Interact()
    {
        isActive = !isActive;
        OnSignalChanged?.Invoke(isActive);
    }

    public bool GetCurrentSignal()
    {
        return isActive;
    }
}
