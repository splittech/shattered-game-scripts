using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignalGenerator
{
    /// <summary>
    /// —рабатывает, когда измен€етс€ передаваемый синал.
    /// </summary>
    /// <remarks>
    /// ѕередаетс€ текущее состо€ние сигнала (вкл./выкл.) и экземпл€р ISignalGenerator. 
    /// </remarks>
    public event Action<bool> OnSignalChanged;
    public bool GetCurrentSignal();
}
