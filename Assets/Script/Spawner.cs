using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float space;
    [SerializeField]
    private float startPosition;
    private static int startMaxRandomValue = 1;
    [SerializeField]
    private int maxRandomValue = startMaxRandomValue;
    [SerializeField]
    private int minRandomValue = 1;
    [SerializeField]
    // row will be 9 and column will 6
    private TwoDimentionalItems obstacles = new TwoDimentionalItems();
    private byte countSpawnOnStart;
    private byte countDoneMoving = 0;
    [SerializeField]
    private byte countInWarning = 0, countNotNullObstacle = 0;
    private bool isDoneMoveUpInArray = false, isDoneMoving = false;
    private bool isDoneWarning = false, isDoneChecking = false;
    private Obstacle[] warningObstacle = null;
    private Obstacle gameoverObstacle = null;
    private bool spawnOnStart = true;
    private static float maxSpawnRate = 0.7f;
    [SerializeField] [Range(0f, 1f)]
    private float spawnRate = 0.25f;
    private float spawnStartRate;
    [SerializeField] [Range(0f, 1f)]
    private float spawnItemRate = 0.25f;
    #region Singleton
    public static Spawner Instance;
    private void Awake()
    {
        spawnStartRate = spawnRate;
        Instance = this;
    }
    #endregion
    #region Properties
    public TwoDimentionalItems Obstacles { get => obstacles; }
    #endregion
    private void Update()
    {
        // Spawn 4 obstacle on start
        switch(GameManager.Instance.gameMode)
        {
            case GameManager.GameMode.survival:
                switch(GameManager.Instance.gameState)
                {
                    case GameManager.GameState.play:
                        if (spawnOnStart)
                        {
                            if (countSpawnOnStart < 4 && GameManager.Instance.isSpawning == false)
                            {
                                GameManager.Instance.turn = 0;
                                minRandomValue = 1;
                                maxRandomValue = 2;
                                countSpawnOnStart++;
                                spawnRate = spawnStartRate;
                                GameManager.Instance.isSpawning = true;
                            }
                            else if (countSpawnOnStart >= 4)
                            {
                                countSpawnOnStart = 0;
                                spawnOnStart = false;
                            }
                        }
                        if (GameManager.Instance.isSpawning)
                        {
                            SpawnOneLine();
                        }
                        break;
                }
                break;
            case GameManager.GameMode.level:
                //To do: load level.
                break;
        }
    }
    private void SpawnOneLine()
    {
        if (isDoneMoveUpInArray == false)
        {
            MoveObstaclesInArrayUp();
            isDoneMoveUpInArray = true;
        }
        if (isDoneMoveUpInArray && isDoneMoving == false)
        {
            MoveObstaclesUp();
            bool isAllStopMoving = countDoneMoving == countNotNullObstacle && countDoneMoving != 0 && countNotNullObstacle != 0;
            if (isAllStopMoving)
                isDoneMoving = true;
        }
        if (isDoneMoving && isDoneChecking == false)
        {
            if(isDoneWarning == false)
            {
                warningObstacle = CheckIsInWarning();
                isDoneWarning = true;
            }
            if (warningObstacle != null && warningObstacle.Length > 0 && isDoneWarning)
            {
                CheckingIsGameOver();
            }
            else
                isDoneChecking = true;
        }
        if (isDoneMoving && isDoneChecking || countNotNullObstacle == 0 && countDoneMoving == 0)
        {
            SpawnItems();
            countNotNullObstacle = 0;
            countDoneMoving = 0;
            isDoneMoving = false;
            isDoneChecking = false;
            isDoneMoveUpInArray = false;
            isDoneWarning = false;
            if (spawnRate >= maxSpawnRate)
                spawnRate = maxSpawnRate;
            else
                spawnRate += 0.02f;
            GameManager.Instance.isSpawning = false;
            GameManager.Instance.isEndTurn = true;
            GameManager.Instance.turn++;
            gameoverObstacle = null;
            warningObstacle = null;
            minRandomValue = maxRandomValue / 2;
            GetItemBackToPoolOnLastRow();
            maxRandomValue = startMaxRandomValue + (int)GameManager.Instance.turn;
            isDoneMoving = false;
        }
    }
    private void SpawnItems()
    {
        int count = 0;
        for(byte column = 0; column < obstacles.rows[0].columns.Count; column++)
        {
            Debug.Log(column);
            //Random that slot is spawn or not.
            int randomSpawn = Random.Range(0, 100);
            if(randomSpawn <= 80)
            {
                float randomRate = Random.Range(0f, 1f);
                if(randomRate <= spawnRate)
                {
                    SpawnObstacle(column);
                    count++;
                }
                
            }
            else
            {
                float randomItemSpawn = Random.Range(0f, 1f);
                if(randomItemSpawn <= spawnItemRate)
                {
                    byte itemCount = 0;
                    for(byte i = 0; i < obstacles.rows[0].columns.Count; i++)
                    {
                        Debug.Log(obstacles.rows[0].columns.Count);
                        if((obstacles.rows[0].columns[i] is AddItem || obstacles.rows[0].columns[i] is SizeItem) && obstacles.rows[0].columns[i] != null)
                        {
                            itemCount++;
                        }
                    }
                    if (itemCount == 0)
                    {
                        SpawnItem(column);
                    }
                }
            }
        }
        if (count == 0)
        {
            //get null index of first row in array
            List<byte> indexes = new List<byte>();
            for(byte i = 0; i < obstacles.rows[0].columns.Count; i++)
            {
                if (obstacles.rows[0].columns[i] == null)
                    indexes.Add(i);
            }
            int randomIndex = Random.Range(0, indexes.Count);
            SpawnObstacle(indexes[randomIndex]);
        }
    }
    private void SpawnObstacle(byte column)
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Obstacles Pool");
        GameObject newObj;
        newObj = GetPooledObjectOrCreateNew(column, pool);
        obstacles.rows[0].columns[column] = newObj.GetComponent<Obstacle>();
        newObj.GetComponent<Obstacle>().HP = (uint)Random.Range(minRandomValue, maxRandomValue);
    }
    private GameObject[] GetItemOnLastRow()
    {
        List<GameObject> result = new List<GameObject>();
        for (int column = 0; column < obstacles.rows[0].columns.Count; column++)
        {
            if (obstacles.rows[obstacles.rows.Count - 1].columns[column] != null && obstacles.rows[obstacles.rows.Count - 1].columns[column] is AddItem)
            {
                result.Add(obstacles.rows[obstacles.rows.Count - 1].columns[column].gameObject);
                obstacles.rows[obstacles.rows.Count - 1].columns[column] = null;
            }
        }
        return result.ToArray();
    }
    private void GetItemBackToPoolOnLastRow()
    {
        GameObject[] itemOnLastRow = GetItemOnLastRow();
        if(itemOnLastRow.Length != 0)
        {
            for(int i = 0; i < itemOnLastRow.Length; i++)
            {
                GameManager.Instance.PoolParty.GetPool("Items Pool").GetBackToPool(itemOnLastRow[i], GameManager.Instance.gameObject.transform.position);
            }
        }
    }
    private GameObject GetPooledObjectOrCreateNew(byte column, Pool pool)
    {
        PoolParty poolParty = GameManager.Instance.PoolParty;
        GameObject[] gameObjectsToPool = pool.ObjectsToPool;
        int randomSprite = Random.Range(0, gameObjectsToPool.Length);
        if (pool.ObjectsPool.Count != 0)
        {
            foreach (GameObject gameObj in pool.ObjectsPool)
            {
                if (gameObj.GetComponent<Obstacle>().Geometry == gameObjectsToPool[randomSprite].GetComponent<Obstacle>().Geometry
                    && gameObj.activeInHierarchy == false)
                {
                    return pool.GetOutOfPool(new Vector2(transform.position.x + startPosition + (space * column), transform.position.y));
                }
            }
        }
        return poolParty.CreateItem(pool, new Vector2(transform.position.x + startPosition + (space * column), transform.position.y), randomSprite, transform);
    }
    private void SpawnItem(byte column)
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Items Pool");
        int randomItem = Random.Range(0, pool.ObjectsToPool.Length);
        GameObject newGameObject;
        if (pool.CanExtend)
        {
            newGameObject = GameManager.Instance.PoolParty.CreateItem(pool, new Vector2(transform.position.x + startPosition + (space * column), transform.position.y), randomItem, transform);
        }
        else
            newGameObject = pool.GetOutOfPool(new Vector2(transform.position.x + startPosition + (space * column), transform.position.y));
        obstacles.rows[0].columns[column] = newGameObject.GetComponent<Item>();
    }
    private void MoveObstaclesInArrayUp()
    {
        for(int row = obstacles.rows.Count - 1; row >= 0; row--)
        {
            for(int column = 0; column < obstacles.rows[0].columns.Count; column++)
            {
                if (obstacles.rows[row].columns[column] != null && row < obstacles.rows.Count - 1)
                {
                    countNotNullObstacle++;
                    if(row + 1 <= obstacles.rows.Count && obstacles.rows[row + 1].columns[column] == null)
                    {
                        obstacles.rows[row + 1].columns[column] = obstacles.rows[row].columns[column];
                        obstacles.rows[row].columns[column] = null;
                    }
                }
            }
        }
    }
    private void MoveObstaclesUp()
    {
        for (int row = 0; row < obstacles.rows.Count; row++)
        {
            for (int column = 0; column < obstacles.rows[0].columns.Count; column++)
            {
                if (obstacles.rows[row].columns[column] != null)
                {
                    obstacles.rows[row].columns[column].Moving(new Vector2(obstacles.rows[row].columns[column].transform.position.x, transform.position.y + (space * row)));
                    if (obstacles.rows[row].columns[column].IsDoneMoving)
                        countDoneMoving++;
                }
            }
        }
    }
    private Obstacle[] CheckIsInWarning()
    {
        List<Obstacle> inWarningObstacles = new List<Obstacle>();
        for(int row = obstacles.rows.Count - 1; row >= 0; row--)
        {
            for(int column = 0; column < obstacles.rows[0].columns.Count; column++)
            {
                //If in row 9 have obstacle => warning by shaking the obstacle in row 9.
                //If in row 10 have obstacle => warning by shaking the obstacle in row 10. ==> GameOver...
                if(row >= obstacles.rows.Count - 2 && obstacles.rows[row].columns[column] != null && obstacles.rows[row].columns[column] is Obstacle)
                {
                    if(row == obstacles.rows.Count - 1)
                    {
                        gameoverObstacle = (Obstacle)obstacles.rows[row].columns[column];
                    }
                    Obstacle obstacle = (Obstacle)obstacles.rows[row].columns[column];
                    obstacle.Shaking();
                    inWarningObstacles.Add(obstacle);
                }
            }
        }
        return inWarningObstacles.ToArray();
    }
    private void CheckingIsGameOver()
    {
        for(int row = Obstacles.rows.Count - 1; row >= 0 ; row--)
        {
            for(int column = Obstacles.rows[0].columns.Count - 1; column >= 0; column--)
            {
                if(Obstacles.rows[row].columns[column] is Obstacle)
                {
                    Obstacle obstacle = (Obstacle)Obstacles.rows[row].columns[column];
                    if(obstacle.IsGameOver == true)
                    {
                        GameManager.Instance.gameState = GameManager.GameState.gameover;
                    }
                }
            }
        }
        isDoneChecking = true;
    }
}
