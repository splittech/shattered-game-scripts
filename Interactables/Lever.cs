using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Lever : MonoBehaviour, ISignalGenerator
{
    [Header("References")]
    Interactive interactive;
    Animator animator;

    public event Action<bool> OnSignalChanged;
    bool isActive = false;

    [Header("Debug")]
    public bool debugEnabled = false;

    private void Awake()
    {
        interactive = GetComponent<Interactive>();

        interactive.OnInteracted += Interact;

        if (TryGetComponent<Animator>(out var animatorComponent))
        {
            animator = animatorComponent;
        }
    }

    void Interact()
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
