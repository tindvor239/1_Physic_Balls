using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    [SerializeField]
    private ShopItem item;
    public bool isUsing = false;
    [SerializeField]
    private GameObject selectedDisplay;
    [SerializeField]
    private GameObject normalDisplay;
    [SerializeField]
    private Text info;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text price;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Button button;
    #region Properties
    public ShopItem ShopItem { get => item; }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        icon.sprite = item.itemObject.icon;
        if(item.IsSold && isUsing == false)
        {
            Sold();
        }
        else if(item.IsSold && isUsing)
        {
            Use();
        }
        else
        {
            OnSale();
        }
    }
    public void OnClick()
    {
        if(item.IsSold && isUsing == false)
        {
            Use();
            GameManager.Instance.ClickSound();
            //change ball sprite
        }
        else if(item.IsSold == false)
        {
            item.Buy();
            if(item.IsSold == true)
            {
                GameManager.Instance.BuySound();
                Sold();
            }
            else
            {
                GameManager.Instance.ErrorSound();
            }
        }
    }
    public void Sold()
    {
        //every ball in game will change images.
        background.sprite = UIShopMenu.Instance.NormalBackground;
        button.image.sprite = UIShopMenu.Instance.NormalButton;
        selectedDisplay.SetActive(true);
        normalDisplay.SetActive(false);
        info.text = "Use";
    }
    public void OnSale()
    {
        background.sprite = UIShopMenu.Instance.NormalBackground;
        button.image.sprite = UIShopMenu.Instance.NormalButton;
        selectedDisplay.SetActive(false);
        normalDisplay.SetActive(true);
        price.text = item.price.ToString("#,##0");
    }
    private void Use()
    {
        isUsing = true;
        UIShopMenu.Instance.UnUseAll();
        background.sprite = UIShopMenu.Instance.SelectedBackground;
        button.image.sprite = UIShopMenu.Instance.SelectedButton;
        item.Use();
        GameManager.Instance.ChangeBallsPrefabSprites(item.itemObject.icon);
        selectedDisplay.SetActive(true);
        normalDisplay.SetActive(false);
        UIShopMenu.Instance.inUseBall = item.itemObject;
        info.text = "In Use";
        Debug.Log("Use");
    }
}
