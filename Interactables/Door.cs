using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Door : SignalsReceiverObject
{
    [Header("References")]
    Animator animator;

    [Header("Debug")]
    public bool debugEnabled;

    private void Awake()
    {
        if (TryGetComponent<Animator>(out var animatorComponent))
        {
            animator = animatorComponent;
        }
    }

    protected override void ChangeState(bool state)
    {
        if (debugEnabled)
        {
            DebugHelper.LogWithObject(gameObject, "state", state.ToString());
        }

        if (animator != null)
        {
            animator.SetBool("isOpened", state);
        }
    }
}
