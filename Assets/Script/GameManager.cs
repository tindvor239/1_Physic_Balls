using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(PoolParty))]
public class GameManager : Singleton<GameManager>
{
    [Header("Save/Load Settings")]
    [SerializeField]
    private List<LevelPackage> levelPackages;
    [SerializeField]
    private Level level;
    [SerializeField]
    private GameObject spawnBall;
    [SerializeField]
    private GameObject gameScene;
    public enum GameMode { survival, level, editor }
    public enum GameState { start, level, play, pause, gameover, win };
    #region Gameplay Setting
    [Header("Gameplay Settings")]
    public GameMode gameMode = GameMode.survival;
    public GameState gameState = GameState.play;
    public uint turn = 0;
    public uint lastTurn = 0;
    public bool isSpawning = false;
    public bool isEndTurn = true;
    [SerializeField]
    private List<Sprite> triangleSprites = new List<Sprite>();
    [SerializeField]
    private List<Sprite> cubeSprites = new List<Sprite>();
    [SerializeField]
    private List<Sprite> circleSprites = new List<Sprite>();
    [SerializeField]
    private List<Sprite> pentagonSprites = new List<Sprite>();
    [SerializeField]
    private List<Color32> colors = new List<Color32>();
    [SerializeField]
    private GameObject[] fans;
    [SerializeField]
    private Collider2D[] frames;
    [SerializeField]
    private float gravity = 4.2f;
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
    public bool firstStart = false;
    [SerializeField]
    private List<Obstacle> hitObstacles = new List<Obstacle>();
    [SerializeField]
    private PoolParty poolParty;
    #endregion
    #region Level Settings
    [Header("Level Settings")]
    [SerializeField]
    private bool isToolBoxActive = false;
    [SerializeField]
    private bool isEditorBoxActive = false;
    [SerializeField]
    private Sprite starOff;
    [SerializeField]
    private Sprite starOn;
    [SerializeField]
    private Sprite winStarOn;
    [SerializeField]
    private Sprite winStarOff;
    [SerializeField]
    private Vector2 winStarSizeOn = new Vector2(160, 150);
    [SerializeField]
    private Vector2 winStarSizeOff = new Vector2(120, 110);
    [SerializeField]
    public LevelButton currentLevel;
    [SerializeField]
    private GameObject levelHolder;
    [SerializeField]
    private List<LevelButton> levelButtons;
    #endregion
    [Header("Prefabs")]
    [SerializeField]
    private GameObject levelPrefab;
    public float timer;
    public bool isReset = false;
    #region Singleton
    protected override void OnAwake()
    {
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
        set
        {
            if(score != null)
            {
                score.text = value.ToString("#,##0");
            }
        }
    }
    private string GetScore()
    {
        string result = "0";
        if(score.text != null)
        {
            string[] splitedString = score.text.Split(',');
            for (int i = 0; i < splitedString.Length; i++)
            {
                result += splitedString[i];
            }
        }
        else
        {
            Debug.LogError("Not have Score text in this " + gameObject.name);
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
    public List<Obstacle> HitObstacles { get => hitObstacles; }
    public GameObject[] Fans { get => fans; }
    public Collider2D[] Frames { get => frames; }
    public List<Sprite> TriangleSprites { get => triangleSprites; }
    public List<Sprite> CubeSprites { get => cubeSprites; }
    public List<Sprite> PentagonSprites { get => pentagonSprites; }
    public List<Sprite> CircleSprites { get => circleSprites; }
    private bool isUpdateOneTime = true;
    public Sprite StarOff { get => starOff; }
    public Sprite StarOn { get => starOn; }
    #endregion

    public delegate void OnUpdateOneTime();
    public event OnUpdateOneTime onUpdateOneTime;
    private void Start()
    {
        //To do show start menu.
        gameState = GameState.start;
    }
    private void Update()
    {
        switch (gameState)
        {
            case GameState.start:
                OnStart();
                break;
            case GameState.level:
                OnLevel();
                break;
            case GameState.play:
                TimeCount();
                if(Input.GetMouseButtonDown(0))
                {
                    firstStart = false;
                }
                OnPlay();
                
                if(onUpdateOneTime != null && isUpdateOneTime && isEndTurn)
                {
                    onUpdateOneTime.Invoke();
                    isUpdateOneTime = false;
                }
                if(isEndTurn == false)
                {
                    isUpdateOneTime = true;
                }
                switch(gameMode)
                {
                    case GameMode.level:

                        break;
                }
                break;
            case GameState.pause:
                OnPause();
                break;
            case GameState.gameover:
                OnGameover();
                break;
            case GameState.win:
                OnWin();
                break;
        }
    }
    public void SetStars()
    {
        UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
        int currentTurnCount = level.TurnCount - ((int)turn - 1);
        float value = (float)currentTurnCount / (float)level.TurnCount;
        gameMenu.Sections[0].Sections[0].gameObject.GetComponent<Slider>().value = value;
        UIMenu levelStars = gameMenu.Sections[0].Sections[1];
        level.UpdateStatus();
        SetStarImages(levelStars.Images, starOn, starOff, level.Stars);
    }
    private void SetStarImages(List<Image> images, Sprite starOn, Sprite starOff, int starCount)
    {
        for(int index = 0; index < images.Count; index++)
        {
            if(index < starCount)
            {
                images[index].sprite = starOn;
            }
            else
            {
                images[index].sprite = starOff;
            }
        }
    }
    public GameObject CreateObject(Transform parent, GameObject prefab)
    {
        return Instantiate(prefab, parent);
    }
    #region Button Behaviors
    public void ChooseLevel()
    {
        //To do: show level menu.
        gameMode = GameMode.level;
        gameState = GameState.level;
        firstStart = true;
        //load every level files.
        List<string> levelInfos = new List<string>();
        bool canCreate = true;
        //load level blocks.
        foreach(LevelPackage levelPackage in levelPackages)
        {
            foreach(LevelButton button in levelButtons)
            {
                if(button.Name == levelPackage.name)
                {
                    canCreate = false;
                }
            }
            if(canCreate)
            {
                GameObject objectLevel = Instantiate(levelPrefab, levelHolder.transform);
                LevelButton levelButton = objectLevel.GetComponent<LevelButton>();
                UIMenu uIMenu = objectLevel.GetComponent<UIMenu>();
                levelButton.Name = levelPackage.name;
                SetStarImages(uIMenu.Images, starOn, StarOff, levelPackage.Stars);
                levelButton.levelPackage = levelPackage;
                levelButtons.Add(levelButton);
            }
            canCreate = true;
        }
    }
    public void ChooseSurvival()
    {
        //To do: show play survival mode
        gameMode = GameMode.survival;
        for (int i = 0; i < Spawner.Instance.Obstacles.rows.Count; i++)
        {
            for (int j = 0; j < Spawner.Instance.Obstacles.rows[i].columns.Count; j++)
            {
                if (Spawner.Instance.Obstacles.rows[i].columns[j] != null)
                {
                    if (Spawner.Instance.Obstacles.rows[i].columns[j].GetComponent<Obstacle>() != null)
                    {
                        poolParty.GetPool("Obstacles Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, transform.position);
                    }
                    else
                    {
                        poolParty.GetPool("Items Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, transform.position);
                    }
                }
            }
        }
        for (int s = 0; s < Level.Balls.Count; s++)
        {
            GameObject destroyObject = Level.Balls[s].gameObject;
            Destroy(destroyObject);
        }
        Level.Balls.Clear();
        Shooter.Instance.Balls.Clear();
        UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
        gameMenu.Sections[0].gameObject.SetActive(false);
        firstStart = true;
        gameState = GameState.play;

        Ball ball = CreateObject(GameScene.transform, Level.BallPrefab).GetComponent<Ball>();
        ball.gameObject.transform.position = SpawnBall.transform.position;
        Shooter.Instance.Balls.Add(ball);
        Shooter.Instance.Reload();
        DoozyUI.UIManager.ShowUiElement("GAMEPLAY_UI");
        Time.timeScale = 1f;
    }
    private void ShowTutorialOnPlay()
    {
        if(firstStart)
        {
            //To do show tutorial.
            DoozyUI.UIManager.ShowUiElement("TUTORIAL_UI");
        }
        else
        {
            DoozyUI.UIManager.HideUiElement("TUTORIAL_UI");
        }
    }
    private void OnPause()
    {
        Time.timeScale = 0;
        DoozyUI.UIManager.ShowUiElement("PAUSE_UI");
        gameState = GameState.pause;
    }
    private void OnStart()
    {
        DoozyUI.UIManager.ShowUiElement("START_UI");
        gameState = GameState.start;
    }
    private void OnWin()
    {
        DoozyUI.UIManager.ShowUiElement("WIN_UI");
        List<DoozyUI.UIElement> uIElements = DoozyUI.UIManager.GetUiElements("WIN_UI");
        switch(gameMode)
        {
            case GameMode.survival:

                //Show Watch video on.
                //if(ZenSDK.instance.IsVideoRewardReady())
                //{
                //    ZenSDK.instance.ShowVideoReward((bool isSuccess) =>
                //    {
                //        if()
                //    });
                //}
                ////Else then lose.

                uIElements[0].gameObject.GetComponent<UIMenu>().Sections[0].gameObject.SetActive(true);
                uIElements[0].gameObject.GetComponent<UIMenu>().Sections[1].gameObject.SetActive(false);
                UIMenu survivalSection = uIElements[0].gameObject.GetComponent<UIMenu>().Sections[0];
                survivalSection.MenuInfos[1].text = score.text;
                survivalSection.MenuInfos[2].text = BestScore.ToString();
                break;
            case GameMode.level:
                UIMenu winMenu = uIElements[0].gameObject.GetComponent<UIMenu>();
                UIMenu levelSection = winMenu.Sections[1];
                levelSection.gameObject.SetActive(true);
                SetStarImages(levelSection.Images, winStarOn, winStarOff, level.Stars);
                //resize star image on/off becuz sprite size of on/off r diffent of each other.
                foreach(Image star in levelSection.Images)
                {
                    RectTransform rectTransform = star.GetComponent<RectTransform>();
                    if(star.sprite == winStarOn)
                    {
                        rectTransform.sizeDelta = winStarSizeOn;
                    }
                    else
                    {
                        rectTransform.sizeDelta = winStarSizeOff;
                    }
                }
                levelSection.MenuInfos[0].text = "Stage: " + level.Name;
                levelSection.MenuInfos[1].text = "Score: " + Score;
                winMenu.Sections[0].gameObject.SetActive(false);
                //storage star in level.
                level.Storage.ConvertedLevel.Save(level);
                break;
        }
    }
    private void OnLevel()
    {
        DoozyUI.UIManager.ShowUiElement("LEVEL_UI");
        gameState = GameState.level;
        UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
        gameMenu.Sections[0].gameObject.SetActive(true);
    }
    private void OnPlay()
    {
        DoozyUI.UIManager.HideUiElement("START_UI");
        DoozyUI.UIManager.ShowUiElement("GAMEPLAY_UI");
        ShowTutorialOnPlay();
    }
    private void OnGameover()
    {
        switch (gameMode)
        {
            case GameMode.survival:
                ZenSDK.instance.ReportScore("Endless", Score);
                DoozyUI.UIManager.ShowUiElement("WIN_UI");
                totalScore.text = Score.ToString();
                SetBestScore();
                bestScore.text = string.Format("BEST: {0}", BestScore.ToString("#,##0"));
                ZenSDK.instance.ShowFullScreen();
                break;
            case GameMode.level:
                //show level sections.
                break;
        }
    }
    //Button Section.
    public void Continue()
    {
        Time.timeScale = 1;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
        DoozyUI.UIManager.HideUiElement("GAMEPLAY_UI");
        gameState = GameState.play;
    }
    public void Win()
    {
        gameState = GameState.win;
    }
    public void Menu()
    {
        gameState = GameState.start;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
        DoozyUI.UIManager.HideUiElement("LEVEL_UI");
        DoozyUI.UIManager.HideUiElement("GAMEPLAY_UI");
    }
    public void Pause()
    {
        gameState = GameState.pause;
    }
    private void DestroyBallsAt(int startIndex)
    {
        List<GameObject> destroyBalls = new List<GameObject>();
        for (int index = startIndex; index < level.Balls.Count; index++)
        {
            GameObject ball = level.Balls[index].gameObject;
            Debug.Log("index: " + index);
            destroyBalls.Add(ball);
        }
        level.Balls.RemoveRange(startIndex, level.Balls.Count - startIndex);
        foreach (GameObject go in destroyBalls)
        {
            Destroy(go);
        }
    }
    public void Restart()
    {
        Time.timeScale = 1;
        switch (gameMode)
        {
            case GameMode.survival:
                DestroyBallsAt(0);
                GetObstaclesInSpawnerToPool();
                Ball ball = CreateObject(GameScene.transform, Level.BallPrefab).GetComponent<Ball>();
                ball.gameObject.transform.position = SpawnBall.transform.position;
                Shooter.Instance.Balls.Add(ball);
                level.Balls[0].GetComponent<SpriteRenderer>().color = Color.white;
                Spawner.Instance.spawnOnStart = true;
                break;
            case GameMode.level:
                Debug.Log("Retried");
                isReset = true;
                Shooter.Instance.isShooting = false;
                currentLevel.OnSelected();
                Spawner.Instance.spawnOnStart = false;
                break;
        }
        Shooter.Instance.isDoneShoot = true;
        Shooter.Instance.Reload();
        Shooter.Instance.ContainBalls = new List<GameObject>();
        firstStart = true;
        isSpawning = false;
        isEndTurn = true;
        Shooter.Instance.isAllIn = true;
        Shooter.Instance.isShooting = false;
        turn = 0;
        timer = 0;
        Score = 0;
        gameState = GameState.play;
        Shooter.Instance.reloadOnEndTurnTime = Shooter.ReloadOnEndTurnDelay;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
    }
    private void GetObstaclesInSpawnerToPool()
    {
        for (int row = 0; row < Spawner.Instance.Obstacles.rows.Count; row++)
        {
            Items items = Spawner.Instance.Obstacles.rows[row];
            for (int column = 0; column < items.columns.Count; column++)
            {
                if (items.columns[column] != null)
                {
                    Item item = items.columns[column];
                    Pool pool = new Pool();
                    if (item is Obstacle)
                    {
                        pool = poolParty.GetPool("Obstacles Pool");
                    }
                    else if (item is AddItem || item is SizeItem)
                    {
                        pool = poolParty.GetPool("Items Pool");
                    }
                    item.BackToPool(pool);
                }
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion
    private void TimeCount()
    {
        timer += Time.deltaTime;
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
        time.text = string.Format("{0} : {1}", minutes, seconds);
    }
    public void SetSpriteColor(Obstacle obstacle)
    {
        List<Sprite> sprites = new List<Sprite>();
        sprites = GetSprites(obstacle.Geometry);
        if(obstacle.HP <= 20)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[0];
            obstacle.Background.color = colors[0];
        }
        else if(obstacle.HP <= 40)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[1];
            obstacle.Background.color = colors[1];
        }
        else if (obstacle.HP <= 60)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[2];
            obstacle.Background.color = colors[2];
        }
        else if (obstacle.HP <= 70)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[3];
            obstacle.Background.color = colors[3];
        }
        else if (obstacle.HP <= 80)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[4];
            obstacle.Background.color = colors[4];
        }
        else if (obstacle.HP <= 90)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[5];
            obstacle.Background.color = colors[5];
        }
        else if (obstacle.HP > 91)
        {
            obstacle.GetComponent<SpriteRenderer>().sprite = sprites[6];
            obstacle.Background.color = colors[6];
        }
    }
    private List<Sprite> GetSprites(Geometry geometry)
    {
        switch(geometry)
        {
            case Geometry.circle:
                return circleSprites;
            case Geometry.cube:
                return cubeSprites;
            case Geometry.triangle:
                return triangleSprites;
            case Geometry.pentagon:
                return pentagonSprites;
            default:
                return null;
        }
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
    public void Tools()
    {
        isToolBoxActive = !isToolBoxActive;
        if(isToolBoxActive)
        {
            DoozyUI.UIElement.ShowUIElement("TOOLS_UI");
        }
        else
        {
            DoozyUI.UIElement.HideUIElement("TOOLS_UI");
        }
    }

    public void Editor()
    {
        isEditorBoxActive = !isEditorBoxActive;
        if(isEditorBoxActive)
        {
            DoozyUI.UIElement.ShowUIElement("EDITOR_UI");
        }
        else
        {
            DoozyUI.UIElement.HideUIElement("EDITOR_UI");
        }
        Debug.Log("In");
    }
    //Reset Levels.
}
