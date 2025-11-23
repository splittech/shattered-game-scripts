using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Door : SignalsReceiverObject
{
    [Header("Door Parameters")]
    public GameObject doorObject;

    [Header("Debug")]
    public bool debugEnabled;
    protected override void ChangeState(bool state)
    {
        if (debugEnabled)
        {
            DebugHelper.LogWithObject(gameObject, "state", state.ToString());
        }
        doorObject.SetActive(!state);
    }
}
