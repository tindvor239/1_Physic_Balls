using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Items
{
    [SerializeField]
    private string name;
    [SerializeField]
    private List<Item> columns = new List<Item>();
    public List<Item> Columns
    {
        get
        {
            if(columns.Count != 0)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columns[i].name = string.Format("column {0}", i);
                }
            }
            return columns;
        }
    }
}

[Serializable]
public class TwoDimentionalItems
{
    [SerializeField]
    public List<Items> rows = new List<Items>();
}