using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Срабатывает, когда изменяется состояние передвижения игрока (стоит, идет, делает рывок, бежит).
    /// </summary>
    /// <remarks>
    /// Передается состояние, в которое перешел игрок.
    /// </remarks>
    public Action<MovementStates> OnPlayerMovementStateChanged;
    /// <summary>
    /// Срабатывает, когда угол между направлением предвижения и взгляда изменяется и пересекает значения в граусах: 45, 135, -45, -135
    /// </summary>
    /// <remarks>
    /// Передается одно из 4-х направлений относительно движеня, в область которого был перемещен взгляд.
    /// </remarks>
    public Action<BaseViewDirections> OnPlayerBaseViewDirectionChanged;
    /// <summary>
    /// Срабатывает, когда игрок доходит до точки назначения.
    /// </summary>
    /// <remarks>
    /// Ничего не передается
    /// </remarks>
    public Action OnPlayerFinishedWalkToPosition;
    /// <summary>
    /// Срабатывает, когда изменяется количество выносливости игрока.
    /// </summary>
    /// <remarks>
    /// Передается текущее значение выносливости. 
    /// </remarks>
    public Action<float> OnStaminaChanged;

    [Header("Walk")]
    public float walkSpeed = 10f;
    public float walkForce = 30f;
    public float walkDrag = 6f;
    Vector3 flooredMoveInput = Vector3.zero;
    Vector3 viewDirection = Vector3.zero;
    float viewMoveAngle = 0f;
    float viewMoveAnglePrevious = 0f;
    bool wasWalking = false;

    [Header("Dash")]
    public float dashForce = 17f;
    public float dashDrag = 4f;
    Vector3 dashDirection = Vector3.zero;
    bool isEndingDash = false;

    [Header("Sprint")]
    public float sprintSpeed = 20f;
    public float sprintForce = 60f;
    bool sprintInput = false;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float dashStaminaConsumption = 20f;
    public float sprintStaminaConsumption = 1f;
    public float idleStaminaReplenishment = 1f;
    float currentStamina = 100;

    [Header("Interactable Objects")]
    public float interactionAvailableRadius = 2f;
    public float finalSnapToInteractionPointDistance = 0.6f;

    [Header("References")]
    Rigidbody rb;
    Camera mainCamera;

    [Header("Input System")]
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction mousePositionAction;
    InputAction dashAction;

    [Header("Ray Cast Floor Layer")]
    public LayerMask interactableLayer = ~0;

    [Header("Debug")]
    public bool debugEnabled = false;

    MovementStates movementState = MovementStates.Idle;

    public enum MovementStates
    {
        Idle,
        Walking,
        Dashing,
        Sprinting,
        WalkingToPosition,
        Interacting
    }

    public enum BaseViewDirections
    {
        Forward,
        Back,
        Right,
        Left
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["WASD"];
        mousePositionAction = playerInput.actions["Point"];
        dashAction = playerInput.actions["Dash"];

        dashAction.started += Dash;
        dashAction.started += StartSprint;
        dashAction.canceled += EndSprint;

        mainCamera = Camera.main;

        GameManager.playerMovement = this;
    }

    void Start()
    {
        //movementState = MovementStates.Idle;
        //OnPlayerMovementStateChanged?.Invoke(movementState);
    }

    void Update()
    {
        GetInput();
        SetRotation();
        CheckState();
        CheckViewMoveAngleChanged();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void Dash(InputAction.CallbackContext context)
    {
        if ((movementState == MovementStates.Idle ||
             movementState == MovementStates.Walking)
             && currentStamina >= dashStaminaConsumption)
        {
            ChangeState(MovementStates.Dashing);
            SpendStamina(dashStaminaConsumption);
            PerformDash();
        }
    }

    void StartSprint(InputAction.CallbackContext context)
    {
        sprintInput = true;
    }

    void EndSprint(InputAction.CallbackContext context)
    {
        sprintInput = false;
    }

    public void WalkToPosition(Vector3 targetPos, Vector3 viewDirection)
    {
        if (movementState != MovementStates.WalkingToPosition)
        {
            ChangeState(MovementStates.WalkingToPosition);
            ChangeBaseViewDirection(BaseViewDirections.Forward);
            
            StartCoroutine(WalkToPositionCoroutine(targetPos, viewDirection));
        }
    }
    IEnumerator WalkToPositionCoroutine(Vector3 targetPos, Vector3 viewDirection)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;

        transform.LookAt(transform.position + direction);

        float distance = (targetPos - transform.position).magnitude;
        while (distance > finalSnapToInteractionPointDistance)
        {
            distance = (targetPos - transform.position).magnitude;
            PerformWalk(direction);
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        transform.LookAt(transform.position + viewDirection);

        ChangeState(MovementStates.Interacting);
        yield return new WaitForSeconds(2f); // Animation time

        ChangeState(MovementStates.Idle);
        OnPlayerFinishedWalkToPosition?.Invoke();
    }

    void GetInput()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();

        // All direction vectors are basically 2D, without hight.
        // So character is moved only on a flat plane, its Y coordinate
        // cannot be changed with this controller.
        flooredMoveInput = new Vector3(moveInput.x, 0f, moveInput.y);
        if (flooredMoveInput != Vector3.zero)
        {
            dashDirection = flooredMoveInput;
        }

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            viewDirection = hit.point - transform.position;
            viewDirection.y = 0f;

            viewMoveAnglePrevious = viewMoveAngle;
            viewMoveAngle = Vector2.SignedAngle(
                new Vector2(dashDirection.x, dashDirection.z),
                new Vector2(viewDirection.x, viewDirection.z)
            );

            if (debugEnabled)
            {
                DebugHelper.ShowDebugRays(transform.position, new Vector3[]
                {
                    viewDirection,
                    transform.forward,
                    dashDirection,
                    flooredMoveInput,
                });
            }
        }
    }

    void CheckState()
    {
        if (movementState == MovementStates.Idle)
        {
            if (flooredMoveInput != Vector3.zero)
            {
                ChangeState(MovementStates.Walking);
            }
        }
        else if (movementState == MovementStates.Walking)
        {
            if (flooredMoveInput == Vector3.zero)
            {
                ChangeState(MovementStates.Idle);
            }
        }
        else if (movementState == MovementStates.Dashing)
        {
            if (rb.velocity.magnitude > 2f)
            {
                isEndingDash = true;
            }

            if (isEndingDash && rb.velocity.magnitude < 0.5f)
            {
                isEndingDash = false;

                if (sprintInput && currentStamina >= sprintStaminaConsumption)
                {
                    ChangeState(MovementStates.Sprinting);
                }
                else
                {
                    rb.velocity = Vector3.zero;

                    if (flooredMoveInput != Vector3.zero)
                    {
                        ChangeState(MovementStates.Walking);
                    }
                    else
                    {
                        ChangeState(MovementStates.Idle);
                    }
                }
            }
        }
        else if (movementState == MovementStates.Sprinting)
        {
            if (!sprintInput)
            {
                if (flooredMoveInput == Vector3.zero)
                {
                    ChangeState(MovementStates.Idle);
                }
                else
                {
                    ChangeState(MovementStates.Walking);
                }
            }
        }
        
    }

    void ChangeState(MovementStates movementState)
    {
        this.movementState = movementState;
        OnPlayerMovementStateChanged?.Invoke(movementState);
    }

    void CheckViewMoveAngleChanged()
    {
        if (movementState != MovementStates.Walking)
        {
            wasWalking = false;
            return;
        }

        float rotatedAngle = (viewMoveAngle + 45) * Mathf.Deg2Rad;
        Vector2 baseViewDirectionVector = new Vector2(
            Mathf.Sign(Mathf.Cos(rotatedAngle)),
            Mathf.Sign(Mathf.Sin(rotatedAngle))
        );

        float rotatedAnglePrevious = (viewMoveAnglePrevious + 45) * Mathf.Deg2Rad;
        Vector2 baseViewDirectionVectorPrevious = new Vector2(
            Mathf.Sign(Mathf.Cos(rotatedAnglePrevious)),
            Mathf.Sign(Mathf.Sin(rotatedAnglePrevious))
        );

        if (baseViewDirectionVector == new Vector2(1f, 1f) &&
           (baseViewDirectionVector != baseViewDirectionVectorPrevious || !wasWalking))
        {
            ChangeBaseViewDirection(BaseViewDirections.Forward);
        }
        else if (baseViewDirectionVector == new Vector2(1f, -1f) &&
                (baseViewDirectionVector != baseViewDirectionVectorPrevious || !wasWalking))
        {
            ChangeBaseViewDirection(BaseViewDirections.Left);
        }
        else if (baseViewDirectionVector == new Vector2(-1f, 1f) &&
                (baseViewDirectionVector != baseViewDirectionVectorPrevious || !wasWalking))
        {
            ChangeBaseViewDirection(BaseViewDirections.Right);
        }
        else if (baseViewDirectionVector == new Vector2(-1f, -1f) &&
                (baseViewDirectionVector != baseViewDirectionVectorPrevious || !wasWalking))
        {
            ChangeBaseViewDirection(BaseViewDirections.Back);
        }

        if (!wasWalking)
        {
            wasWalking = true;
        }
    }

    void ChangeBaseViewDirection(BaseViewDirections baseViewDirection)
    {
        OnPlayerBaseViewDirectionChanged?.Invoke(baseViewDirection);
    }

    void SetRotation()
    {
        if (movementState == MovementStates.Idle ||
            movementState == MovementStates.Walking)
        {
            transform.LookAt(transform.position + viewDirection);
        }
        if (movementState == MovementStates.Sprinting)
        {
            transform.LookAt(transform.position + dashDirection);
        }
    }

    void ApplyMovement()
    {
        if (movementState == MovementStates.Sprinting)
        {
            if (currentStamina < sprintStaminaConsumption)
            {
                EndSprint(new InputAction.CallbackContext());
            }
            else
            {
                SpendStamina(sprintStaminaConsumption);
                PerformSprint();
            }
        }
        if (movementState == MovementStates.Walking)
        {
            PerformWalk(flooredMoveInput);
        }
        if (movementState == MovementStates.Idle)
        {
            PerformIdle();
        }
    }

    void PerformIdle()
    {
        ReplenishStamina(idleStaminaReplenishment);
    }

    void PerformWalk(Vector3 direction)
    {
        rb.AddForce(direction * walkForce, ForceMode.Acceleration);
        rb.drag = walkDrag;

        if (rb.velocity.magnitude > walkSpeed)
        {
            rb.velocity = rb.velocity.normalized * walkSpeed;
        }
    }

    void PerformDash()
    {
        transform.LookAt(transform.position + dashDirection);

        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        rb.drag = dashDrag;
    }

    void PerformSprint()
    {
        rb.AddForce(dashDirection * sprintForce, ForceMode.Acceleration);
        rb.drag = walkDrag;

        if (rb.velocity.magnitude > sprintSpeed)
        {
            rb.velocity = rb.velocity.normalized * sprintSpeed;
        }
    }

    void SpendStamina(float ammount)
    {
        if (currentStamina == 0)
        {
            return;
        }

        if (currentStamina - ammount < 0)
        {
            currentStamina = 0;
        }
        else
        {
            currentStamina -= ammount;
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }

    void ReplenishStamina(float ammount)
    {
        if (currentStamina == maxStamina)
        {
            return;
        }

        if (currentStamina + ammount > maxStamina)
        {
            currentStamina = maxStamina;
        }
        else
        {
            currentStamina += ammount;
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }
}
