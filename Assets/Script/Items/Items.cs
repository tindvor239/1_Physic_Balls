using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Items
{
    [SerializeField]
    private string name;
    public List<Item> columns = new List<Item>();
}

[Serializable]
public class TwoDimentionalItems
{
    [SerializeField]
    public List<Items> rows = new List<Items>();
}