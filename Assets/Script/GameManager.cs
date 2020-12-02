using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> balls;
    public enum GameState {play, gameover};
    [SerializeField]
    public GameState gameState = GameState.play;
    public uint turn = 0;
    public uint lastTurn = 0;
    public bool isSpawning = false;
    public bool isEndTurn = true;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject gameoverMenu;
    [SerializeField]
    private Text comment;
    [SerializeField]
    private Text totalScore;
    [SerializeField]
    private Text bestScore;
    [SerializeField]
    private Text score;
    [SerializeField]
    private Text time;

    private float timer;
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        Score = 0;
    }
    #endregion
    #region Properties
    public List<GameObject> Balls { get => balls; }
    public int Score
    {
        get => int.Parse(score.text);
        set => score.text = value.ToString();
    }
    public int BestScore
    {
        get => PlayerPrefs.GetInt("Best Score");
        private set
        {
            PlayerPrefs.SetInt("Best Score", value);
        }
    }
    private string Comment
    {
        get => comment.text;
        set => comment.text = value;
    }
    #endregion

    private void Update()
    {
        switch(gameState)
        {
            case GameState.play:
                TimeCount();
                OnPlay();
                break;
            case GameState.gameover:
                OnGameover();
                break;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        gameState = GameState.play;
        SceneManager.LoadScene(0);
    }
    private void TimeCount()
    {
        timer += Time.deltaTime;
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
        time.text = string.Format("TIME\n{0} : {1}", minutes, seconds);
    }
    private void OnPlay()
    {
        gameoverMenu.SetActive(false);
    }
    private void OnGameover()
    {
        menu.SetActive(true);
        gameoverMenu.SetActive(true);
        totalScore.text = Score.ToString();
        SetBestScore();
        bestScore.text = string.Format("BEST: {0}", BestScore.ToString("#,##0"));
    }
    private void SetBestScore()
    {
        bool isNewHighScore = Score > BestScore;
        if(isNewHighScore)
        {
            BestScore = Score;
            Comment = "Congrat, New High Score!";
        }
        else
        {
            Comment = "Great!";
        }
    }
}
