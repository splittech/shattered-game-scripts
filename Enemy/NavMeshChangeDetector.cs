using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshChangeDetector : MonoBehaviour
{
    public Action OnNavMeshChanged;

    public Door[] doors;

    private void Awake()
    {
        GameManager.navMeshChangeDetector = this;
    }

    private void Start()
    {
        foreach (Door door in doors)
        {
            door.OnNavMeshChanged += NavMeshChanged;
        }
    }

    void NavMeshChanged()
    {
        OnNavMeshChanged?.Invoke();
    }
}
