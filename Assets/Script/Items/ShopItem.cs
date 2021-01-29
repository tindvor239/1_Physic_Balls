using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Shop/SaleItem")]
public class ShopItem : ScriptableObject
{
    public ItemObject itemObject;
    public int price;
    public bool isSold;
    public bool inUse;
    #region Properties
    public bool IsSold
    {
        get
        {
            PlayerPrefs.SetInt($"{itemObject.name} isSold", isSold == true ? 1 : 0);
            return isSold;
        }
    }
    public bool InUse
    {
        get
        {
            int boolean = 0;
            PlayerPrefs.GetInt($"{itemObject.name} isUse", boolean);
            return isSold = boolean == 1 ? true : false;
        }
    }
    #endregion
    public void Buy()
    {
        if(UIShopMenu.Instance.Money >= price)
        {
            UIShopMenu.Instance.Money -= price;
            isSold = true;
        }
    }
    public void Use()
    {
        inUse = true;
        PlayerPrefs.SetInt($"{itemObject.name} inUse", inUse == true ? 1 : 0);
    }
    public void UnUse()
    {
        inUse = false;
        PlayerPrefs.SetInt($"{itemObject.name} inUse", inUse == true ? 1 : 0);
    }
}
