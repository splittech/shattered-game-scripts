using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerParametersUI : MonoBehaviour
{
    public PlayerMovement player;
    public Slider staminaSlider;
    public TMP_Text staminaText;

    float maxStamina = 0;

    private void Start()
    {
        player = GameManager.playerMovement;

        player.OnStaminaChanged += ChangeStamina;

        maxStamina = player.maxStamina;
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
        staminaSlider.value = stamina / maxStamina;
        staminaText.text = ((int)stamina).ToString();
    }
}
