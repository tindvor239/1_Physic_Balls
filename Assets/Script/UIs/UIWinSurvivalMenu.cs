using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinSurvivalMenu : Singleton<UIWinSurvivalMenu>
{
    [SerializeField]
    private Text comment;
    [SerializeField]
    private Text score;
    [SerializeField]
    private Text highScore;
    #region Properties
    public Text Comment { get => comment; }
    public Text Score { get => score; }
    public Text HighScore { get => highScore; }
    #endregion
    public void ShowUI(string comment, int score, int highScore)
    {
        DoozyUI.UIElement.ShowUIElement("SURVIVAL_WIN_UI");
        this.comment.text = comment;
        this.score.text = score.ToString();
        this.highScore.text = highScore.ToString("#,##0");
    }
    public void HideUI()
    {
        DoozyUI.UIElement.HideUIElement("SURVIVAL_WIN_UI");
    }
}
