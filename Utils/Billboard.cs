using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        // Простой поворот к камере
        transform.LookAt(transform.position + _camera.transform.forward);
    }
}
