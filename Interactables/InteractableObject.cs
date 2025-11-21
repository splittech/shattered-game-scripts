using System;
using UnityEngine;

[RequireComponent(typeof(HoverableObject))]
[RequireComponent(typeof(Outline))]
public abstract class InteractableObject : MonoBehaviour
{
    [Header("Parameters")]
    public Color reachableColor = Color.green;
    public Color unreachableColor = Color.yellow;
    public InteractionPlaceOptions interactionPlaceType = InteractionPlaceOptions.Fixed;
    float interactionPlaceDistance;
    bool isHovered;

    [Header("References")]
    public Transform interactionPlace;
    PlayerMovement player;
    HoverableObject hoverable;
    Outline outline;

    public enum InteractionPlaceOptions
    {
        Fixed,
        DependsOnPlayerPosition
    }

    private void Awake()
    {
        hoverable = GetComponent<HoverableObject>();
        outline = GetComponent<Outline>();

        hoverable.OnClicked += GoToInteractionPos;
        hoverable.OnHovered += MouseStateChanged;

        if (outline.enabled)
        {
            outline.enabled = false;
        }
        outline.OutlineColor = unreachableColor;
    }

    private void Start()
    {
        player = GameManager.playerMovement;

        interactionPlaceDistance = (interactionPlace.transform.position - transform.position).magnitude;
    }

    void MouseStateChanged(bool isHovered)
    {
        this.isHovered = isHovered;
        outline.enabled = isHovered;
    }

    void UpdateOutlineColor(Color color)
    {
        if (outline.OutlineColor != color)
        {
            outline.OutlineColor = color;
        }
    }

    float playerDistance = float.MaxValue;

    private void Update()
    {
        // Наверное лучше не проверять флаг каждый раз для каждого объекта,
        // а подписываться на событие смены позиции персонажа.
        if (isHovered)
        {
            if (interactionPlaceType == InteractionPlaceOptions.DependsOnPlayerPosition)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                interactionPlace.position = transform.position + direction * interactionPlaceDistance;
            }

            playerDistance = (interactionPlace.position - player.transform.position).magnitude;

            if (playerDistance > player.interactionAvailableRadius)
            {
                UpdateOutlineColor(unreachableColor);
            }
            else
            {
                UpdateOutlineColor(reachableColor);
            }
        }
    }

    void GoToInteractionPos()
    {
        if (playerDistance > player.interactionAvailableRadius)
        {
            return;
        }

        player.OnPlayerFinishedWalkToPosition += StartInteraction;

        Vector3 viewDirection = (transform.position - interactionPlace.position).normalized;
        viewDirection.y = 0;

        player.WalkToPosition(
            interactionPlace.position,
            viewDirection
        );
    }

    void StartInteraction()
    {
        GameManager.playerMovement.OnPlayerFinishedWalkToPosition -= Interact;
        Interact();
    }

    public abstract void Interact();
}
