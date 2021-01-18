using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private new Text name;
    public LevelPackage levelPackage;
    [SerializeField]
    private Image lockImage;
    private bool isLock = false;
    private int count = 0;
    #region Properties
    public string Name { get => name.text; set => name.text = value; }
    public LevelPackage LevelPackage { get => levelPackage; }
    #endregion
    public void OnSelected()
    {
        if(isLock == false)
        {
            for (int i = 0; i < Spawner.Instance.Obstacles.rows.Count; i++)
            {
                for (int j = 0; j < Spawner.Instance.Obstacles.rows[i].columns.Count; j++)
                {
                    if (Spawner.Instance.Obstacles.rows[i].columns[j] != null)
                    {
                        if(Spawner.Instance.Obstacles.rows[i].columns[j].GetComponent<Obstacle>() != null)
                        {
                            GameManager.Instance.PoolParty.GetPool("Obstacles Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, GameManager.Instance.transform.position);
                        }
                        else
                        {
                            GameManager.Instance.PoolParty.GetPool("Items Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, GameManager.Instance.transform.position);
                        }
                    }
                }
            }
            for (int s = 0; s < GameManager.Instance.Level.Balls.Count; s++)
            {
                GameObject destroyObject = GameManager.Instance.Level.Balls[s].gameObject;
                Destroy(destroyObject);
            }
            ResetLevel();
        }
    }
    public void Lock()
    {
        name.gameObject.SetActive(false);
        if(GetComponent<UIMenu>() != null)
        {
            foreach(Image image in GetComponent<UIMenu>().Images)
            {
                image.gameObject.SetActive(false);
            }
        }
        lockImage.gameObject.SetActive(true);
        isLock = true;
    }
    public void Unlock()
    {
        name.gameObject.SetActive(true);
        if(GetComponent<UIMenu>() != null)
        {
            foreach(Image image in GetComponent<UIMenu>().Images)
            {
                image.gameObject.SetActive(true);
            }
        }
        lockImage.gameObject.SetActive(false);
        isLock = false;
    }
    private void ResetLevel()
    {
        GameManager.Instance.Level.Balls.Clear();
        Shooter.Instance.Balls.Clear();
        GameManager.Instance.Level.Load(levelPackage);
        GameManager.Instance.SetStars();
        GameManager.Instance.currentLevel = this;
        DoozyUI.UIManager.HideUiElement("LEVEL_UI");
        Time.timeScale = 1f;
        GameManager.Instance.State = GameManager.GameState.play;
        Shooter.Instance.isDoneShoot = true;
        Shooter.Instance.Reload();
        GameManager.Instance.isSpawning = false;
        GameManager.Instance.isEndTurn = true;
        Shooter.Instance.isShooting = false;
        Shooter.Instance.isAllIn = true;
        GameManager.Instance.turn = 0;
        GameManager.Instance.timer = 0;
        GameManager.Instance.Score = 0;
    }
}
