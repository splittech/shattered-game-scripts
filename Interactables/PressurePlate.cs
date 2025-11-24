using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlate : MonoBehaviour, ISignalGenerator
{
    [Header("Pressure Plate Parameters")]
    public float resetTime = 5;
    public bool dealDamage = false;
    public float damageAmmount = 10;
    float currentTime = 5;
    bool isActive = false;

    [Header("Debug")]
    public bool debugEnabled = false;
    public GameObject debugFloatingTextPrefab;
    TMP_Text debugFloatingText;

    public event Action<bool> OnSignalChanged;

    public bool GetCurrentSignal()
    {
        return isActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.playerTag))
        {
            if (dealDamage)
            {
                PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
                playerCombat.TakeDamage(damageAmmount);
            }

            ChangeState(true);
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

    void ChangeState(bool newState)
    {
        if (isActive != newState)
        {
            if (newState)
            {
                currentTime = resetTime;
            }

            isActive = newState;

            if (debugEnabled)
            {
                DebugHelper.LogWithObject(gameObject, "isActive", isActive.ToString());
            }

            OnSignalChanged?.Invoke(isActive);
        }
    }
}
