using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    EnemyMovement enemyMovement;
    Animator animator;
    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();

        enemyMovement.OnStateChanged += ChangeAnimations;
    }

    void ChangeAnimations(EnemyMovement.States state)
    {
        if (state == EnemyMovement.States.Attacking)
        {
            animator.SetTrigger("Attack");
        }
    }
}
