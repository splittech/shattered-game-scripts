using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : InteractableObject, ISignalGenerator
{
    public event Action<bool> OnSignalChanged;

    [Header("Button Parameters")]
    public float resetTime = 5;
    float currentTime = 5;
    bool isActive = false;


    private void FixedUpdate()
    {
        if (isActive)
        {
            currentTime -= Time.fixedDeltaTime;
            if (currentTime < 0)
            {
                ChangeState(false);
            }
        }
    }
    protected override void Interact()
    {
        currentTime = resetTime;
        ChangeState(true);
    }

    void ChangeState(bool newState)
    {
        if (isActive != newState)
        {
            isActive = newState;
            OnSignalChanged?.Invoke(isActive);
        }
    }

    public bool GetCurrentSignal()
    {
        return isActive;
    }
}
