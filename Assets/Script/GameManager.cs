using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Linq;

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
    public enum GameState { start, level, play, pause, gameover, win, shop };
    #region Gameplay Setting
    [Header("Gameplay Settings")]
    [SerializeField]
    private GameMode gameMode = GameMode.survival;
    [SerializeField]
    private GameState gameState = GameState.play;
    public uint turn = 0;
    public uint lastTurn = 0;
    public bool isSpawning = false;
    public bool isEndTurn = true;
    [SerializeField]
    private List<Sprite> isoscelesTriangles = new List<Sprite>();
    [SerializeField]
    private List<Sprite> rightTriangles = new List<Sprite>();
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
    [SerializeField]
    private List<Image> muteIcons = new List<Image>();
    [SerializeField]
    private Sprite muteOn, muteOff;
    [SerializeField]
    private Text muteText;
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
    [SerializeField]
    private AudioClip hitObstacleSound;
    [SerializeField]
    private AudioClip outSound;
    [SerializeField]
    private AudioClip hitItemSound;
    [SerializeField]
    private AudioClip clickSound;
    [SerializeField]
    private AudioClip buySound;
    [SerializeField]
    private AudioClip errorSound;
    [SerializeField]
    private AudioSource audioSource;
    public float speedupTimer = 3f;
    private float speedupDelay = 3f;
    private bool isSpeedUp = false;
    [SerializeField]
    private GameObject starMinX, starMaxX;
    [SerializeField]
    private Button speedUp;
    #endregion
    [Header("Prefabs")]
    [SerializeField]
    private GameObject levelPrefab;
    public float timer;
    public bool isReset = false;
    private bool isClickedLevel = false, isInteractable = true;
    private bool onStateChange = false, onModeChange = false;
    private int gameoverCount = 0;
    private bool isLastSpeedUpBool = false;
    #region Singleton
    protected override void OnAwake()
    {
        poolParty = GetComponent<PoolParty>();
        Score = 0;
    }
    #endregion
    #region Properties
    public GameState State
    {
        get
        {
            return gameState;
        }
        set
        {
            gameState = value;
            onStateChange = true;
        }
    }
    public GameMode Mode
    {
        get
        {
            return gameMode;
        }
        set
        {
            gameMode = value;
            onModeChange = true;
        }
    }
    public Level Level { get => level; }
    public GameObject SpawnBall { get => spawnBall; }
    public GameObject GameScene { get => gameScene; }
    public bool Mute
    {
        set
        {
            PlayerPrefs.SetInt("mute", value == false ? 0 : 1);
        }
        get
        {
            return PlayerPrefs.GetInt("mute") == 0 ? false : true;
        }
    }
    public int Score
    {
        get => int.Parse(GetScore());
        set
        {
            if (score != null)
            {
                score.text = value.ToString("#,##0");
            }
        }
    }
    private string GetScore()
    {
        string result = "0";
        if (score.text != null)
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
    public int HighScore
    {
        get => PlayerPrefs.GetInt("HighScore");
        private set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }
    public ParticleSystem Particle { get => particle; }
    public AudioClip HitObstacleSound { get => hitObstacleSound; }
    public AudioClip HitItemSound { get => hitItemSound; }
    public AudioClip OutSound { get => outSound; }
    public PoolParty PoolParty { get => poolParty; }
    public AudioSource Audio { get => audioSource; }
    public float Gravity { get => gravity; }
    public List<Obstacle> HitObstacles { get => hitObstacles; }
    public GameObject[] Fans { get => fans; }
    public Collider2D[] Frames { get => frames; }
    public List<Sprite> IsoscelesSprites { get => isoscelesTriangles; }
    public List<Sprite> RightTriangles { get => rightTriangles; }
    public List<Sprite> CubeSprites { get => cubeSprites; }
    public List<Sprite> PentagonSprites { get => pentagonSprites; }
    public List<Sprite> CircleSprites { get => circleSprites; }
    private bool isUpdateOneTime = true;
    public Sprite StarOff { get => starOff; }
    public Sprite StarOn { get => starOn; }
    public Button SpeedUp { get => speedUp; }
    public float SpeedUpDelay { get => speedupDelay; }
    #endregion
    public delegate void OnUpdateOneTime();
    public event OnUpdateOneTime onUpdateOneTime;
    private void Start()
    {
        //To do show start menu.
        State = GameState.start;

        if(Mute)
        {
            foreach(Image icon in muteIcons)
            {
                icon.sprite = muteOn;
            }
        }
        else
        {
            foreach (Image icon in muteIcons)
            {
                icon.sprite = muteOff;
            }
        }
        SetTextMute(muteText);
    }
    private void Update()
    {
        switch (State)
        {
            case GameState.start:
                OnStart();
                break;
            case GameState.level:
                OnLevel();
                break;
            case GameState.play:
                TimeCount();
                if (Input.GetMouseButtonDown(0))
                {
                    firstStart = false;
                }
                OnPlay();

                if (onUpdateOneTime != null && isUpdateOneTime && isEndTurn)
                {
                    onUpdateOneTime.Invoke();
                    isUpdateOneTime = false;
                }
                if (isEndTurn == false)
                {
                    if(speedUp.gameObject.activeInHierarchy == false)
                    {
                        speedupTimer -= Time.deltaTime;
                        if(speedupTimer <= 0)
                        {
                            speedUp.gameObject.SetActive(true);
                            speedupTimer = speedupDelay;
                        }
                    }
                    isUpdateOneTime = true;
                }
                switch (gameMode)
                {
                    case GameMode.level:

                        break;
                }
                break;
            case GameState.pause:
                OnPause();
                break;
            case GameState.gameover:
                if (onStateChange)
                {
                    OnGameover();
                }
                break;
            case GameState.win:
                if (onStateChange)
                {
                    OnWin();
                }
                break;
        }
        onStateChange = false;
    }
    public void SetStars()
    {
        UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
        int currentTurnCount = level.TurnCount - (int)turn;
        float value = (float)currentTurnCount / (float)level.TurnCount;
        gameMenu.Sections[0].Sections[0].gameObject.GetComponent<Slider>().value = value;
        UIMenu levelStars = gameMenu.Sections[0].Sections[1];
        CalculateStarPos(levelStars.Images);
        level.UpdateStatus();
        SetStarImages(levelStars.Images, starOn, starOff, level.Stars);
    }
    private void CalculateStarPos(List<Image> stars)
    {
        float maxPos = 100f, minxPos = -100;
        for(int i = 0; i < level.Points.Length; i++)
        {
            float percent = (float)level.Points[i] / (float)level.TurnCount;
            float posX = percent * (maxPos * 2f);
            if(percent < 50f)
            {
                posX = minxPos + posX;
            }
            else
            {
                posX = maxPos - posX;
            }
            Debug.Log("X: " + posX);
            stars[i].transform.localPosition = new Vector2(starMinX.transform.position.x + posX, stars[i].transform.position.y);
        }
    }
    private void SetStarImages(List<Image> images, Sprite starOn, Sprite starOff, int starCount)
    {
        for (int index = 0; index < images.Count; index++)
        {
            if (index < starCount)
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
        if(isInteractable)
        {
            //To do: show level menu.
            Mode = GameMode.level;
            State = GameState.level;
            firstStart = true;
            //load every level files.
            List<string> levelInfos = new List<string>();
            //load level blocks.
            if (isClickedLevel == false)
            {
                foreach (LevelPackage levelPackage in levelPackages)
                {
                    CreateLevelButton(levelPackage);
                }
                LockOnStart();
                SetUnlockLevelButtons();
                foreach(LevelButton button in levelButtons)
                {
                    if (button.LevelPackage != null)
                    {
                        SetStarImages(button.GetComponent<UIMenu>().Images, starOn, StarOff, button.levelPackage.Stars);
                    }
                }
                isClickedLevel = true;
            }

            Time.timeScale = 1f;
            Warning.Instance.StopBlinking();
            speedUp.gameObject.SetActive(false);
            speedupTimer = speedupDelay;
            isInteractable = false;
        }
    }
    public void ClickSound()
    {
        if (Mute == false)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    public void BuySound()
    {
        if(Mute == false)
        {
            audioSource.PlayOneShot(buySound);
        }
    }
    public void ErrorSound()
    {
        if(Mute == false)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }
    public void OnClickMute()
    {
        Mute = !Mute;
        if(Mute)
        {
            foreach(Image icon in muteIcons)
            {
                icon.sprite = muteOn;
            }
        }
        else
        {
            foreach (Image icon in muteIcons)
            {
                icon.sprite = muteOff;
            }
        }
        SetTextMute(muteText);
    }
    public void ChangeBallsPrefabSprites(Sprite sprite)
    {
        level.BallPrefab.GetComponent<SpriteRenderer>().sprite = sprite;
        SpriteRenderer[] cursors = Shooter.Instance.AimCursor.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer cursor in cursors)
        {
            cursor.sprite = sprite;
        }
    }
    private void LockOnStart()
    {
        for (int index = 1; index < levelPackages.Count; index++)
        {
            if (levelButtons[index].LevelPackage != null)
            {
                levelButtons[index].Lock();
            }
        }
    }
    private void SetUnlockLevelButtons()
    {
        for (int index = 0; index < levelPackages.Count; index++)
        {
            if (levelButtons[index].LevelPackage != null)
            {
                if(levelButtons[index].LevelPackage.Stars > 0 && index + 1 < levelPackages.Count)
                {
                    levelButtons[index + 1].Unlock();
                }
            }
        }
    }
    private void CreateLevelButton(LevelPackage levelPackage)
    {
        GameObject objectLevel = Instantiate(levelPrefab, levelHolder.transform);
        LevelButton levelButton = objectLevel.GetComponent<LevelButton>();
        if(levelPackage != null)
        {
            levelButton.Unlock();
            UIMenu uIMenu = objectLevel.GetComponent<UIMenu>();
            levelButton.Name = levelPackage.name;
            SetStarImages(uIMenu.Images, starOn, StarOff, levelPackage.Stars);
            levelButton.levelPackage = levelPackage;
        }
        else
        {
            levelButton.Lock();
        }
        levelButtons.Add(levelButton);
    }
    public void ChooseSurvival()
    {
        //To do: show play survival mode
        if(isInteractable)
        {
            gameMode = GameMode.survival;
            //Clear all existed obstacles.
            for (int i = 0; i < Spawner.Instance.Obstacles.rows.Count; i++)
            {
                for (int j = 0; j < Spawner.Instance.Obstacles.rows[i].columns.Count; j++)
                {
                    if (Spawner.Instance.Obstacles.rows[i].columns[j] != null)
                    {
                        if (Spawner.Instance.Obstacles.rows[i].columns[j].GetComponent<Obstacle>() != null)
                        {
                            poolParty.GetPool("Obstacles Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, transform.position);
                            Spawner.Instance.Obstacles.rows[i].columns[j] = null;
                        }
                        else
                        {
                            poolParty.GetPool("Items Pool").GetBackToPool(Spawner.Instance.Obstacles.rows[i].columns[j].gameObject, transform.position);
                            Spawner.Instance.Obstacles.rows[i].columns[j] = null;
                        }
                    }
                }
            }
            for (int s = 0; s < Level.Balls.Count; s++)
            {
                GameObject destroyObject = Level.Balls[s].gameObject;
                Destroy(destroyObject);
            }
            //Reset row and column.
            Items[] items = new Items[10];
            for(int index = 0; index < items.Length; index++)
            {
                items[index] = new Items();
                items[index].Name = string.Format("row {0}", index);
                Item[] item = new Item[6];
                items[index].columns = item.ToList();
            }
            Spawner.Instance.Obstacles.rows = items.ToList();
            //Clear all balls list
            Level.Balls.Clear();
            Shooter.Instance.Balls.Clear();

            UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
            gameMenu.Sections[0].gameObject.SetActive(false);
            firstStart = true;
            State = GameState.play;
            //Spawn 1 Ball
            Ball ball = CreateObject(GameScene.transform, Level.BallPrefab).GetComponent<Ball>();
            ball.gameObject.transform.position = SpawnBall.transform.position;
            Shooter.Instance.Balls.Add(ball);
            //Reset everythings.
            //bool isContinue = true;
            //if (isContinue == false)
            //{
            firstStart = true;
            isSpawning = false;
            isEndTurn = true;
            Shooter.Instance.isAllIn = true;
            Shooter.Instance.isShooting = false;
        
            Warning.Instance.StopBlinking();
            speedUp.gameObject.SetActive(false);
            speedupTimer = speedupDelay;

            turn = 0;
            timer = 0;
            Score = 0;
            Shooter.Instance.isDoneShoot = true;
            Shooter.Instance.Reload();
            Spawner.Instance.spawnOnStart = true;
            DoozyUI.UIManager.ShowUiElement("GAMEPLAY_UI");
            Time.timeScale = 1f;
            //}
            //else
            //{
            isInteractable = false;
            //}
        }
    }
    public void OnClickSpeedUp()
    {
        Time.timeScale = 2.5f;
        Warning.Instance.Blinking(0, true);
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
        //State = GameState.pause;
    }
    private void OnStart()
    {
        DoozyUI.UIManager.ShowUiElement("START_UI");
        //State = GameState.start;
    }
    private void OnWin()
    {
        switch(gameMode)
        {
            case GameMode.survival:
                break;
            case GameMode.level:
                UIWinLevelMenu.Instance.ShowUI(currentLevel.levelPackage.name, Score);
                UIShopMenu.Instance.Money += Score;
                SetStarImages(UIWinLevelMenu.Instance.Stars, winStarOn, winStarOff, level.Stars);

                //resize star image on/off becuz sprite size of on/off r diffent of each other.
                foreach(Image star in UIWinLevelMenu.Instance.Stars)
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
                currentLevel.Unlock();
                foreach (LevelButton button in levelButtons)
                {
                    if (button.LevelPackage != null)
                    {
                        SetStarImages(button.GetComponent<UIMenu>().Images, starOn, StarOff, button.levelPackage.Stars);
                    }
                }
                //storage star in level.
                level.Storage.ConvertedLevel.Save(level);
                break;
        }
    }
    private void OnLevel()
    {
        DoozyUI.UIManager.ShowUiElement("LEVEL_UI");
        State = GameState.level;
        UIMenu gameMenu = DoozyUI.UIManager.GetUiElements("GAMEPLAY_UI")[0].gameObject.GetComponent<UIMenu>();
        gameMenu.Sections[0].gameObject.SetActive(true);
    }
    private void OnPlay()
    {
        DoozyUI.UIManager.HideUiElement("START_UI");
        UIWinLevelMenu.Instance.HideUI();
        UIWinSurvivalMenu.Instance.HideUI();
        DoozyUI.UIManager.ShowUiElement("GAMEPLAY_UI");
        ShowTutorialOnPlay();
    }
    private void OnGameover()
    {
        switch (gameMode)
        {
            case GameMode.survival:
                ZenSDK.instance.ReportScore("Endless", Score);
                UIWinSurvivalMenu.Instance.ShowUI(Comment(), Score, HighScore);
                Focusing.Instance.gameObject.SetActive(true);
                UIShopMenu.Instance.Money += Score;
                break;
            case GameMode.level:
                //show level sections.
                DoozyUI.UIManager.ShowUiElement("GAMEOVER_UI");
                break;
        }
        gameoverCount++;
        if(gameoverCount >= 4)
        {
            ZenSDK.instance.ShowFullScreen();
        }
        Debug.Log("Gameover Count: " + gameoverCount);
    }
    //Button Section.
    public void Continue()
    {
        Time.timeScale = 1;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
        DoozyUI.UIManager.HideUiElement("GAMEOVER_UI");
        DoozyUI.UIManager.HideUiElement("GAMEPLAY_UI");
        State = GameState.play;
        Warning.Instance.gameObject.SetActive(isLastSpeedUpBool);
    }
    private void NextLevel()
    {
        for(int index = 0; index < levelPackages.Count; index++)
        {
            if(levelPackages[index] == level.Storage.ConvertedLevel && index + 1 < levelPackages.Count)
            {
                levelButtons[index + 1].Unlock();
                levelButtons[index + 1].OnSelected();
                return;
            }
        }
    }
    public void Win()
    {
        State = GameState.win;
    }
    private void SetTextMute(Text text)
    {
        if(Mute)
        {
            text.text = "Sound Off";
        }
        else
        {
            text.text = "Sound On";
        }
    }
    public void Menu()
    {
        State = GameState.start;
        isInteractable = true;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
        DoozyUI.UIManager.HideUiElement("LEVEL_UI");
        DoozyUI.UIManager.HideUiElement("GAMEPLAY_UI");
    }
    public void Pause()
    {
        State = GameState.pause;
        isLastSpeedUpBool = true;
        Warning.Instance.gameObject.SetActive(false);
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
        Shooter.Instance.Balls.RemoveRange(startIndex, Shooter.Instance.Balls.Count - startIndex);
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
                isReset = true;
                Shooter.Instance.isShooting = false;
                if(currentLevel.levelPackage.Stars > 0)
                {
                    NextLevel();
                }
                else
                {
                    currentLevel.OnSelected();
                }
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

        Time.timeScale = 1f;
        Warning.Instance.StopBlinking();
        speedUp.gameObject.SetActive(false);
        speedupTimer = speedupDelay;

        turn = 0;
        timer = 0;
        Score = 0;
        State = GameState.play;
        isInteractable = true;
        Shooter.Instance.reloadOnEndTurnTime = Shooter.ReloadOnEndTurnDelay;
        DoozyUI.UIManager.HideUiElement("PAUSE_UI");
        DoozyUI.UIManager.HideUiElement("GAMEOVER_UI");
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
        switch(gameMode)
        {
            case GameMode.survival:
                if(obstacle.HP <= 5)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[0];
                    obstacle.Background.color = colors[0];
                }
                else if(obstacle.HP <= 10)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[1];
                    obstacle.Background.color = colors[1];
                }
                else if (obstacle.HP <= 20)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[2];
                    obstacle.Background.color = colors[2];
                }
                else if (obstacle.HP <= 30)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[3];
                    obstacle.Background.color = colors[3];
                }
                else if (obstacle.HP <= 40)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[4];
                    obstacle.Background.color = colors[4];
                }
                else if (obstacle.HP <= 50)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[5];
                    obstacle.Background.color = colors[5];
                }
                else if (obstacle.HP > 51)
            {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[6];
                obstacle.Background.color = colors[6];
            }
                break;
            case GameMode.level:
                if (obstacle.HP <= 2)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[0];
                    obstacle.Background.color = colors[0];
                }
                else if (obstacle.HP <= 4)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[1];
                    obstacle.Background.color = colors[1];
                }
                else if (obstacle.HP <= 6)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[2];
                    obstacle.Background.color = colors[2];
                }
                else if (obstacle.HP <= 8)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[3];
                    obstacle.Background.color = colors[3];
                }
                else if (obstacle.HP <= 10)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[4];
                    obstacle.Background.color = colors[4];
                }
                else if (obstacle.HP <= 12)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[5];
                    obstacle.Background.color = colors[5];
                }
                else if (obstacle.HP > 13)
                {
                    obstacle.MainImage.GetComponent<SpriteRenderer>().sprite = sprites[6];
                    obstacle.Background.color = colors[6];
                }
                break;
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
            case Geometry.isoscelesTriangle:
                return isoscelesTriangles;
            case Geometry.rightTriangle:
                return rightTriangles;
            case Geometry.pentagon:
                return pentagonSprites;
            default:
                return null;
        }
    }
    private string Comment()
    {
        bool isNewHighScore = Score > HighScore;
        if(isNewHighScore)
        {
            HighScore = Score;
            return "Congrat, New High Score!";
        }
        else
        {
            return "Great!";
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
    public void OnApplicationQuit()
    {
        
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
