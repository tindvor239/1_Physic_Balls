using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField]
    private int baseStat;
    #region Properties
    public int BaseStat { get => baseStat; }
    #endregion
    public void SetBase(int value)
    {
        baseStat = value;
    }
}
