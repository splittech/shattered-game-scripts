using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerMovement controller;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerMovement>();
        controller.OnPlayerMovementStateChanged += UpdateAnimations;
        controller.OnPlayerBaseViewDirectionChanged += UpdateRotation;
    }

    void UpdateAnimations(PlayerMovement.MovementStates state)
    {
        switch (state)
        {
            case PlayerMovement.MovementStates.Idle:
                animator.SetTrigger("Idle");
                break;
            case PlayerMovement.MovementStates.Dashing:
                animator.SetTrigger("Dash");
                break;
            case PlayerMovement.MovementStates.Sprinting:
                animator.SetTrigger("Sprint");
                break;
            case PlayerMovement.MovementStates.Interacting:
                animator.SetTrigger("Interaction");
                break;
        }
    }

    void UpdateRotation(PlayerMovement.BaseViewDirections viewDirection)
    {
        switch (viewDirection)
        {
            case PlayerMovement.BaseViewDirections.Forward:
                animator.SetTrigger("WalkForward");
                break;
            case PlayerMovement.BaseViewDirections.Back:
                animator.SetTrigger("WalkBack");
                break;
            case PlayerMovement.BaseViewDirections.Right:
                animator.SetTrigger("WalkRight");
                break;
            case PlayerMovement.BaseViewDirections.Left:
                animator.SetTrigger("WalkLeft");
                break;
        }
    }
}
