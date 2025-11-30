using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Interactive))]
public class Button : MonoBehaviour, ISignalGenerator
{
    public event Action<bool> OnSignalChanged;

    [Header("References")]
    Interactive interactive;
    Animator animator;

    [Header("Button Parameters")]
    public float resetTime = 5;
    float currentTime = 5;
    bool isActive = false;

    [Header("Debug")]
    public bool debugEnabled = false;
    public GameObject debugFloatingTextPrefab;
    TMP_Text debugFloatingText;

    private void Awake()
    {
        interactive = GetComponent<Interactive>();

        interactive.OnInteracted += Interact;

        if (TryGetComponent<Animator>(out var animatorComponent))
        {
            animator = animatorComponent;
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (debugEnabled)
            {
                debugFloatingText = DebugHelper.FloatingText(
                    Math.Round(currentTime, 1).ToString(),
                    gameObject,
                    debugFloatingText,
                    debugFloatingTextPrefab
                );
            }
            
            currentTime -= Time.fixedDeltaTime;
            if (currentTime < 0)
            {
                ChangeState(false);
            }
        }
    }
    void Interact()
    {
        currentTime = resetTime;
        ChangeState(true);
    }

    void ChangeState(bool newState)
    {
        if (isActive != newState)
        {
            isActive = newState;

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
    }

    void ChangeAnimationState()
    {
        animator.SetBool("isActive", isActive);
    }

    public bool GetCurrentSignal()
    {
        return isActive;
    }
}
