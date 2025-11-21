using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMouseInteraction : MonoBehaviour
{
    [Header("References")]
    Camera mainCamera;
    HoverableObject currentHoveredObject;

    [Header("Input System")]
    PlayerInput playerInput;
    InputAction mousePositionAction;
    InputAction mouseClickAction;

    [Header("Ray Cast Interactable Layer")]
    public LayerMask interactableLayer = ~0;

    public void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mousePositionAction = playerInput.actions["Point"];
        mouseClickAction = playerInput.actions["Click"];

        mouseClickAction.started += PerformAction;

        mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateHoveredObject();
    }

    void UpdateHoveredObject()
    {
        Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            if (!hit.collider.gameObject.TryGetComponent<HoverableObject>(out var hittedGameobject))
            {
                if (currentHoveredObject != null)
                {
                    currentHoveredObject.MouseUnhover();
                    currentHoveredObject = null;
                }
            }
            else
            {
                if (hittedGameobject != currentHoveredObject)
                {
                    if (currentHoveredObject != null)
                    {
                        currentHoveredObject.MouseUnhover();
                    }
                    currentHoveredObject = hittedGameobject;
                    currentHoveredObject.MouseHover();
                }
            }
        }
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.PerformAction();
        }
    }
}
