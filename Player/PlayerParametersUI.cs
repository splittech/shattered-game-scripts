using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerParametersUI : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public Slider staminaSlider;
    public Slider healthSlider;
    public TMP_Text staminaText;
    public TMP_Text healthText;

    private void Start()
    {
        playerMovement = GameManager.playerMovement;
        playerCombat = GameManager.playerCombat;

        playerMovement.OnStaminaChanged += ChangeStamina;
        playerCombat.OnHealthChanged += ChangeHealth;
    }

    private void ChangeHealth(float currentHealth)
    {
        healthSlider.value = currentHealth / playerCombat.maxHealth;
        healthText.text = ((int)currentHealth).ToString();
    }

    public void ChangeState(PlayerMovement.MovementStates state)
    {
        if (state == PlayerMovement.MovementStates.Idle)
        {
            Debug.Log("Idle");
        }
        else if (state == PlayerMovement.MovementStates.Walking)
        {
            Debug.Log("Moving");
        }
        else if (state == PlayerMovement.MovementStates.Dashing)
        {
            Debug.Log("Dashing");
        }
        else if (state == PlayerMovement.MovementStates.Sprinting)
        {
            Debug.Log("Sprinting");
        }
    }

    public void ChangeStamina(float stamina)
    {
        staminaSlider.value = stamina / playerMovement.maxStamina;
        staminaText.text = ((int)stamina).ToString();
    }
}
