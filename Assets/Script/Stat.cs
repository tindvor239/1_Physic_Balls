using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField]
    private uint baseStat;
    #region Properties
    public uint BaseStat { get => baseStat; }
    #endregion
    public void SetBase(uint value)
    {
        baseStat = value;
    }
}
