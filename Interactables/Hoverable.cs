using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Hoverable : MonoBehaviour
{
    /// <summary>
    /// Срабатывает, когда на объект кликнули мышкой.
    /// </summary>
    /// <remarks>
    /// Ничего не передается.
    /// </remarks>
    public UnityEvent OnClicked;
    /// <summary>
    /// Срабатывает, когда на объект навели или убрали курсор.
    /// </summary>
    /// <remarks>
    /// Передается текущее состояние объекта (наведен курсор или нет).
    /// </remarks>
    public UnityEvent<bool> OnHovered;

    public void MouseHover()
    {
        OnHovered?.Invoke(true);
    }

    public void MouseUnhover()
    {
        OnHovered?.Invoke(false);
    }
    
    public void PerformAction()
    {
        OnClicked?.Invoke();
    }
}
