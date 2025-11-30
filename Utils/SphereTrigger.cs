using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SphereTrigger : MonoBehaviour
{
    public Action<bool> OnPlayerEntered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.playerTag))
        {
            OnPlayerEntered?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameManager.playerTag))
        {
            OnPlayerEntered?.Invoke(false);
        }
    }
}
