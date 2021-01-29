using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinLevelMenu : Singleton<UIWinLevelMenu>
{
    [SerializeField]
    private Text stage;
    [SerializeField]
    private Text score;
    [SerializeField]
    private List<Image> stars = new List<Image>();
    #region Properties
    public List<Image> Stars { get => stars; }
    #endregion
    public void ShowUI(string levelName, int score)
    {
        DoozyUI.UIElement.ShowUIElement("LEVEL_WIN_UI");
        this.stage.text = $"Stage {levelName}";
        this.score.text = score.ToString();
    }
    public void HideUI()
    {
        DoozyUI.UIElement.HideUIElement("LEVEL_WIN_UI");
    }
}
