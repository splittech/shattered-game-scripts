using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactive : MonoBehaviour
{
    [Header("Parameters")]
    public Color reachableColor = Color.green;
    public Color unreachableColor = Color.yellow;
    public InteractionPlaceOptions interactionPlaceType = InteractionPlaceOptions.Fixed;
    float interactionPlaceDistance;
    bool isHovered;

    [Header("References")]
    public Transform interactionPlace;
    PlayerMovement playerMovement;
    Outline outline;
    protected Animator animator;


    public UnityEvent OnInteracted;

    public enum InteractionPlaceOptions
    {
        Fixed,
        DependsOnPlayerPosition
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline.enabled)
        {
            outline.enabled = false;
        }
        outline.OutlineColor = unreachableColor;

        if (TryGetComponent<Animator>(out var animatorComponent))
        {
            animator = animatorComponent;
        }

        interactionPlaceDistance = (interactionPlace.transform.position - transform.position).magnitude;
    }

    private void Start()
    {
        playerMovement = GameManager.playerMovement;
    }

    public void MouseStateChanged(bool isHovered)
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
                Vector3 direction = (playerMovement.transform.position - transform.position).normalized;
                interactionPlace.position = transform.position + direction * interactionPlaceDistance;
            }

            playerDistance = (interactionPlace.position - playerMovement.transform.position).magnitude;

            if (playerDistance > playerMovement.interactionAvailableRadius)
            {
                UpdateOutlineColor(unreachableColor);
            }
            else
            {
                UpdateOutlineColor(reachableColor);
            }
        }
    }

    public void GoToInteractionPos()
    {
        if (playerDistance > playerMovement.interactionAvailableRadius)
        {
            return;
        }

        playerMovement.OnPlayerFinishedWalkToPosition += StartInteraction;

        Vector3 viewDirection = (transform.position - interactionPlace.position).normalized;
        viewDirection.y = 0;

        playerMovement.WalkToPosition(
            interactionPlace.position,
            viewDirection
        );
    }

    void StartInteraction()
    {
        playerMovement.OnPlayerFinishedWalkToPosition -= StartInteraction;
        OnInteracted?.Invoke();
    }
}
