using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Action<States> OnStateChanged;

    [Header("References")]
    public SphereTrigger lostTrigger;
    public SphereTrigger detectionTrigger;
    public SphereTrigger attackTrigger;
    GameObject targetPlayer;
    NavMeshAgent navMesh;
    NavMeshChangeDetector navMeshChangeDetector;

    [Header("Parameters")]
    public Transform[] patrolTransforms;
    public float pathReachableDistance = 20;
    public float lostRadius;
    public float detectionRadius;
    public float attackRadius;
    int currentPatrolTransformIndex = 0;

    public States state = States.Idle;
    public enum States
    {
        Idle,
        Patroling,
        Chasing,
        Attacking
    }

    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.SetDestination(patrolTransforms[currentPatrolTransformIndex].position);
    }

    private void Start()
    {
        SphereCollider detectionCollider = detectionTrigger.GetComponent<SphereCollider>();
        detectionCollider.center = Vector3.zero;
        detectionCollider.radius = detectionRadius;

        SphereCollider attackCollider = attackTrigger.GetComponent<SphereCollider>();
        attackCollider.center = Vector3.zero;
        attackCollider.radius = attackRadius;

        detectionTrigger.OnPlayerEntered += PlayerDetected;
        attackTrigger.OnPlayerEntered += PlayerInAttackRadius;

        targetPlayer = GameManager.playerMovement.gameObject;

        navMeshChangeDetector = GameManager.navMeshChangeDetector;
        navMeshChangeDetector.OnNavMeshChanged += NavMeshChanged;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (state == States.Patroling)
        {
            if (HasReachedDestination())
            {
                navMesh.isStopped = false;
                currentPatrolTransformIndex = (currentPatrolTransformIndex + 1) % patrolTransforms.Length;
                navMesh.SetDestination(patrolTransforms[currentPatrolTransformIndex].position);
            }
        }
        else if (state == States.Chasing)
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(targetPlayer.transform.position);
        }
        else if (state == States.Attacking)
        {
            navMesh.isStopped = true;
        }
    }

    bool HasReachedDestination()
    {
        return !navMesh.pathPending && navMesh.remainingDistance <= navMesh.stoppingDistance;
    }

    void PlayerInAttackRadius(bool entered)
    {
        if (state != States.Attacking && entered)
        {
            ChangeState(States.Attacking);
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        float playerDistance = (targetPlayer.transform.position - transform.position).magnitude;
        if (playerDistance > detectionRadius)
        {
            ChangeState(States.Patroling);
        }
        else
        {
            ChangeState(States.Chasing);
        }
    }

    void ChangeState(States newState)
    {
        if (state != newState)
        {
            state = newState;
            DebugHelper.LogWithObject(gameObject, "State", state.ToString());
            OnStateChanged?.Invoke(state);
        }
    }

    void NavMeshChanged()
    {
        if (playerInReach)
        {
            if (state == States.Idle || state == States.Patroling)
            {
                if (!IsPathTooLong(targetPlayer.transform.position))
                {
                    ChangeState(States.Chasing);
                }
            }
        }
    }

    bool playerInReach = false;

    void PlayerDetected(bool enetered)
    {
        if (state == States.Idle || state == States.Patroling && enetered)
        {
            if (!IsPathTooLong(targetPlayer.transform.position))
            {
                ChangeState(States.Chasing);
            }
        }

        if (state == States.Chasing && !enetered)
        {
            ChangeState(States.Patroling);
        }

        playerInReach = enetered;
    }

    bool IsTargetVisible(Vector3 targetPosition)
    {
        return true;
    }

    bool IsPathTooLong(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            float pathLength = CalculatePathLength(path);
            return pathLength > pathReachableDistance;
        }

        return true;
    }


    float CalculatePathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2) return 0f;

        float length = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }
}
