using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// —рабатывает, когда измен€етс€ количество очков жизни игрока.
    /// </summary>
    /// <remarks>
    /// ѕередаетс€ текущее значение очков жизни. 
    /// </remarks>
    public Action<float> OnHealthChanged;

    public float maxHealth = 100f;
    float currentHealth;

    private void Awake()
    {
        GameManager.playerCombat = this;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float ammount)
    {
        Debug.Log(ammount);
        if (currentHealth == 0)
        {
            return;
        }

        if (currentHealth - ammount < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth -= ammount;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    public void Heal(float ammount)
    {
        if (currentHealth == maxHealth)
        {
            return;
        }

        if (currentHealth + ammount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += ammount;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}
