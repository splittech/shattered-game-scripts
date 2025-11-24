using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject, ISignalGenerator
{
    public event Action<bool> OnSignalChanged;
    bool isActive = false;

    [Header("Debug")]
    public bool debugEnabled = false;
    protected override void Interact()
    {
        isActive = !isActive;

        if (debugEnabled)
        {
            DebugHelper.LogWithObject(gameObject, "isActive", isActive.ToString());
        }

        if (animator != null)
        {
            ChangeAnimationState();
        }

        OnSignalChanged?.Invoke(isActive);
    }

    public bool GetCurrentSignal()
    {
        return isActive;
    }

    void ChangeAnimationState()
    {
        animator.SetBool("isActive", isActive);
    }
}
