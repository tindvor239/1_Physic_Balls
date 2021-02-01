using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopMenu : Singleton<UIShopMenu>
{
    [Header("UI Resource")]
    [SerializeField]
    private Sprite selectedBackground;
    [SerializeField]
    private Sprite normalBackground;
    [SerializeField]
    private Sprite selectedButton;
    [SerializeField]
    private Sprite normalButton;
    [SerializeField]
    private Sprite selectedBall;
    [SerializeField]
    private Sprite selectedStar;
    [SerializeField]
    private Sprite unSelectedBall;
    [SerializeField]
    private Sprite unSelectedStar;
    [Header("UI Handler")]
    [SerializeField]
    private Button ball;
    [SerializeField]
    private Button star;
    [SerializeField]
    private List<UIShopItem> uIShopItems = new List<UIShopItem>();
    [SerializeField]
    private Text money;
    [SerializeField]
    private GameObject ballsSection;
    [SerializeField]
    private GameObject starSection;
    public ItemObject inUseBall;
    #region Properties
    public Sprite NormalBackground { get => normalBackground; }
    public Sprite SelectedBackground { get => selectedBackground; }
    public Sprite SelectedButton { get => selectedButton; }
    public Sprite NormalButton { get => normalButton; }
    public List<UIShopItem> UIShopItems { get => uIShopItems; }
    public int Money
    {
        get
        {
            try
            {
                money.text = PlayerPrefs.GetInt("money").ToString();
                return PlayerPrefs.GetInt("money");
            }
            catch(Exception)
            {
                return 0;
            }
        }
        set
        {
            money.text = value.ToString();
            PlayerPrefs.SetInt("money", value);
        }
    }
    #endregion
    private void Start()
    {
        money.text = Money.ToString();
    }
    public void BallSelected()
    {
        ball.image.sprite = selectedBall;
        star.image.sprite = unSelectedStar;
        ballsSection.SetActive(true);
        starSection.SetActive(false);
    }
    public void StarSelected()
    {
        ball.image.sprite = unSelectedBall;
        star.image.sprite = selectedStar;
        ballsSection.SetActive(false);
        starSection.SetActive(true);
    }

    public void Show()
    {
        GameManager.Instance.State = GameManager.GameState.shop;
        DoozyUI.UIElement.ShowUIElement("SHOP_UI");
        DoozyUI.UIElement.HideUIElement("TUTORIAL_UI");
        DoozyUI.UIElement.HideUIElement("START_UI");
    }
    public void Hide()
    {
        GameManager.Instance.State = GameManager.GameState.start;
        DoozyUI.UIElement.HideUIElement("SHOP_UI");
        DoozyUI.UIElement.HideUIElement("TUTORIAL_UI");
        DoozyUI.UIElement.ShowUIElement("START_UI");
    }
    public void UnUseAll()
    {
        foreach(UIShopItem item in uIShopItems)
        {
            if(item.ShopItem.IsSold)
            {
                item.Sold();
                item.isUsing = false;
                item.ShopItem.UnUse();
            }
            else
            {
                item.OnSale();
            }
        }
    }

    public void OnClickWatch()
    {
        //Show Watch video on.
        if (ZenSDK.instance.IsVideoRewardReady())
        {
            ZenSDK.instance.ShowVideoReward((bool isSuccess) =>
            {
                if(isSuccess)
                {
                    GiveCoin(150);
                    GameManager.Instance.Continue();
                }
            });
        }
        //GiveCoin(150);
        //GameManager.Instance.Continue();
    }

    private void GiveCoin(int amount)
    {
        Money += amount;
    }
}
