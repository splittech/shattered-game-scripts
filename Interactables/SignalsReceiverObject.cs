using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SignalsReceiverObject : MonoBehaviour
{
    public List<ISignalGenerator> signalGenerators;

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

        if (checkSignalsOption == CheckSignalsOptions.AllSignalsAreTrue)
        {
            newState = activeSignals == signalGenerators.Count;
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
            ChangeState(isActive);
        }
    }

    void ChangeState(bool state)
    {

    }
}
