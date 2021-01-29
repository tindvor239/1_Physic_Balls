using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item")]
public class ItemObject : ScriptableObject
{
    public new string name;
    public Sprite icon;
}