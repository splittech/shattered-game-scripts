using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class SignalsReceiver : MonoBehaviour
{
    public GameObject[] signalGeneratorObjects;
    public ISignalGenerator[] signalGenerators;
    public UnityEvent<bool> OnSignalChanged;

    void OnValidate()
    {
        if (signalGeneratorObjects != null)
        {
            foreach (var signalGeneratorObject in signalGeneratorObjects)
            {
                if (signalGeneratorObject != null)
                {
                    if (!signalGeneratorObject.TryGetComponent<ISignalGenerator>(out var checkImplementsInterface))
                        Debug.LogError($"Object does not implement the interface {typeof(ISignalGenerator)}", this);
                }
            }
        }
    }

    bool isActive = false;

    int activeSignals;

    public CheckSignalsOptions checkSignalsOption = CheckSignalsOptions.AllSignalsAreTrue;
    public enum CheckSignalsOptions
    {
        AllSignalsAreTrue,
        AtLeastOneSignalIsTrue,
        OnlyOneSignalIsTrue
    }

    private void Start()
    {
        signalGenerators = new ISignalGenerator[signalGeneratorObjects.Length];
        for (int i = 0; i < signalGeneratorObjects.Length; i++)
        {
            signalGenerators[i] = signalGeneratorObjects[i].GetComponent<ISignalGenerator>();
        }

        foreach (var signalGenerator in signalGenerators)
        {
            signalGenerator.OnSignalChanged += CheckUpdatedSignals;
        }
        CheckStartSignals();
    }

    void CheckUpdatedSignals(bool signal)
    {
        if (signal)
        {
            activeSignals++;
        }
        else
        {
            activeSignals--;
        }

        CheckState();
    }

    void CheckStartSignals()
    {
        foreach (var signalGenerator in signalGenerators)
        {
            if (signalGenerator.GetCurrentSignal())
            {
                activeSignals++;
            }
        }
        CheckState();
    }

    void CheckState()
    {
        bool newState = false;

        if (signalGeneratorObjects.Length == 0)
        {
            return;
        }

        if (checkSignalsOption == CheckSignalsOptions.AllSignalsAreTrue)
        {
            newState = activeSignals == signalGenerators.Length;
        }
        else if (checkSignalsOption == CheckSignalsOptions.AtLeastOneSignalIsTrue)
        {
            newState = activeSignals != 0;
        }
        else if (checkSignalsOption == CheckSignalsOptions.OnlyOneSignalIsTrue)
        {
            newState = activeSignals == 1;
        }

        if (newState != isActive)
        {
            isActive = newState;
            DebugHelper.LogWithObject(gameObject, "isActive", isActive.ToString());
            OnSignalChanged?.Invoke(isActive);
        }
    }
}
