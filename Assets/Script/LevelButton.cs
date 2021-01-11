using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private new Text name;
    public LevelPackage levelPackage;
    #region Properties
    public string Name { get => name.text; set => name.text = value; }
    public LevelPackage LevelPackage { get => levelPackage; }
    #endregion
    public void OnSelected()
    {
        GameManager.Instance.Level.Load(levelPackage);
        GameManager.Instance.SetStars();
        GameManager.Instance.currentLevel = this;
        DoozyUI.UIManager.HideUiElement("LEVEL_UI");
        Time.timeScale = 1f;
        GameManager.Instance.gameState = GameManager.GameState.play;
    }
}
