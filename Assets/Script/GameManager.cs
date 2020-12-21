using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PoolParty))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Level level;
    [SerializeField]
    private GameObject spawnBall;
    [SerializeField]
    private GameObject gameScene;
    public enum GameMode { survival, level }
    public enum GameState {play, gameover};
    [SerializeField]
    public GameMode gameMode = GameMode.survival;
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
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    private List<GameObject> obstaclesPool;
    [SerializeField]
    private List<ParticleSystem> particlesPool;
    [SerializeField]
    private PoolParty poolParty;
    [SerializeField]
    private float gravity = 4.2f;
    [SerializeField]
    private string levelFolder = "Level";
    private float timer;
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        poolParty = GetComponent<PoolParty>();
        Score = 0;
    }
    #endregion
    #region Properties
    public Level Level { get => level; }
    public GameObject SpawnBall { get => spawnBall; }
    public GameObject GameScene { get => gameScene; }
    public int Score
    {
        get => int.Parse(GetScore());
        set => score.text = value.ToString("#,##0");
    }
    public string LevelFolder { get => levelFolder; }
    private string GetScore()
    {
        string[] splitedString = score.text.Split(',');
        string result = "";
        for(int i = 0; i < splitedString.Length; i++)
        {
            result += splitedString[i];
        }
        return result;
    }
    public int BestScore
    {
        get => PlayerPrefs.GetInt("Best Score");
        private set
        {
            PlayerPrefs.SetInt("Best Score", value);
        }
    }
    public ParticleSystem Particle { get => particle; }
    public PoolParty PoolParty { get => poolParty; }
    private string Comment
    {
        get => comment.text;
        set => comment.text = value;
    }
    public float Gravity { get => gravity; }
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
    public GameObject CreateObject(Transform parent, GameObject prefab)
    {
        return Instantiate(prefab, parent);
    }
    public void ChooseLevel()
    {
        //To do: show level menu.
    }
    public void ChooseSurvival()
    {
        //To do: show play survival mode
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
        time.text = string.Format("{0} : {1}", minutes, seconds);
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
