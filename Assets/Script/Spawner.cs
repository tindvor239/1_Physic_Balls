using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float space;
    [SerializeField]
    private float startPosition;
    [SerializeField]
    private GameObject[] itemPrefabs;
    [SerializeField]
    private GameObject[] obstaclePrefabs;
    private static int startMaxRandomValue = 1;
    [SerializeField]
    private int maxRandomValue = startMaxRandomValue;
    [SerializeField]
    private int minRandomValue = 1;
    private Item[,] obstacles = new Item[9, 6];
    private byte countSpawnOnStart;
    private byte countDoneMoving = 0, countDoneShaking = 0;
    [SerializeField]
    private byte countInWarning = 0, countNotNullObstacle = 0;
    private bool isDoneMoveUpInArray = false, isDoneMoving = false;
    private bool isInWarning = false, isDoneChecking = false;
    private bool spawnOnStart = true;
    private static byte maxSpawnRate = 70;
    [SerializeField]
    private byte spawnRate = 25;
    private byte spawnStartRate;
    [SerializeField]
    private byte spawnItemRate = 25;
    private void Awake()
    {
        spawnStartRate = spawnRate;
    }
    private void Update()
    {
        // Spawn 4 obstacle on start
        if (spawnOnStart)
        {
            if (countSpawnOnStart < 4 && GameManager.Instance.isSpawning == false)
            {
                GameManager.Instance.turn = 0;
                minRandomValue = 1;
                maxRandomValue = 1;
                countSpawnOnStart++;
                spawnRate = spawnStartRate;
                GameManager.Instance.isSpawning = true;
            }
            else if(countSpawnOnStart >= 4)
            {
                countSpawnOnStart = 0;
                spawnOnStart = false;
            }
        }
        if (GameManager.Instance.isSpawning)
        {
            SpawnOneLine();
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
            isInWarning = CheckIsInWarning();
            if(isInWarning)
            {
                CheckingIsGameOver();
                isInWarning = false;
            }
            if(countDoneShaking == 0 && isInWarning == false || countDoneShaking != 0 && countNotNullObstacle != 0 && countDoneShaking == countNotNullObstacle )
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
            if (spawnRate >= maxSpawnRate)
                spawnRate = maxSpawnRate;
            else
                spawnRate++;
            GameManager.Instance.isSpawning = false;
            GameManager.Instance.isEndTurn = true;
            GameManager.Instance.turn++;
            if (maxRandomValue - 6 > 0)
            {
                Debug.Log(maxRandomValue - 6);
                minRandomValue = maxRandomValue - 6;
            }
            maxRandomValue = startMaxRandomValue + (int)GameManager.Instance.turn;
            isDoneMoving = false;
        }
    }
    private void SpawnItems()
    {
        int count = 0;
        for(byte column = 0; column < obstacles.GetLength(1); column++)
        {
            //Random that slot is spawn or not.
            int randomSpawn = Random.Range(0, 100);
            Debug.Log("chance: "+ randomSpawn);
            if(randomSpawn <= 80)
            {
                int randomRate = Random.Range(0, 100);
                if(randomRate <= spawnRate)
                {
                    SpawnObstacle(column);
                    count++;
                }
                
            }
            else
            {
                int randomItemSpawn = Random.Range(0, 100);
                Debug.Log("item chance: " + randomItemSpawn);
                if(randomItemSpawn <= spawnItemRate)
                {
                    byte itemCount = 0;
                    for(byte i = 0; i < obstacles.GetLength(1); i++)
                    {
                        if((obstacles[0, i] is AddItem || obstacles[0, i] is SizeItem) && obstacles[0, i] != null)
                        {
                            Debug.Log("Have Item In " + obstacles[0, i]);
                            itemCount++;
                        }
                    }
                    if (itemCount == 0)
                    {
                        Debug.Log("In");
                        SpawnItem(column);
                    }
                }
            }
        }
        if (count == 0)
        {
            byte randomIndex = (byte)Random.Range(0, 6);
            SpawnObstacle(randomIndex);
        }
    }
    private void SpawnObstacle(byte column)
    {
        int randomSprite = Random.Range(0, obstaclePrefabs.Length);
        GameObject newGameObject = Instantiate(obstaclePrefabs[randomSprite], transform);
        obstacles[0, column] = newGameObject.GetComponent<Obstacle>();
        newGameObject.GetComponent<Obstacle>().HitCount = (uint)Random.Range(minRandomValue, maxRandomValue);
        newGameObject.transform.position = new Vector2(transform.position.x + startPosition + (space * column), transform.position.y);
    }
    private void SpawnItem(byte column)
    {
        int randomItem = Random.Range(0, itemPrefabs.Length);
        GameObject newGameObject = Instantiate(itemPrefabs[randomItem], transform);
        obstacles[0, column] = newGameObject.GetComponent<Item>();
        newGameObject.transform.position = new Vector2(transform.position.x + startPosition + (space * column), transform.position.y);
    }
    private void MoveObstaclesInArrayUp()
    {
        for(int row = obstacles.GetLength(0) - 1; row >= 0; row--)
        {
            for(int column = 0; column < obstacles.GetLength(1); column++)
            {
                if (obstacles[row, column] != null)
                {
                    countNotNullObstacle++;
                    if(row + 1 <= obstacles.GetLength(0) && obstacles[row + 1, column] == null)
                    {
                        obstacles[row + 1, column] = obstacles[row, column];
                        obstacles[row, column] = null;
                    }
                }
            }
        }
    }
    private void MoveObstaclesUp()
    {
        for (int row = 0; row < obstacles.GetLength(0); row++)
        {
            for (int column = 0; column < obstacles.GetLength(1); column++)
            {
                if (obstacles[row, column] != null)
                {
                    obstacles[row, column].Moving(new Vector2(obstacles[row, column].transform.position.x, transform.position.y + (space * row)));
                    if (obstacles[row, column].IsDoneMoving)
                        countDoneMoving++;
                }
            }
        }
    }
    private bool CheckIsInWarning()
    {
        for(int row = obstacles.GetLength(0) - 1; row >= 0; row--)
        {
            for(int column = 0; column <obstacles.GetLength(1); column++)
            {
                //If in row 9 have obstacle => warning by shaking the obstacle in row 9.
                //If in row 10 have obstacle => warning by shaking the obstacle in row 10. ==> GameOver...
                if(row >= obstacles.GetLength(0) - 2 && obstacles[row, column] != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void CheckingIsGameOver()
    {
        for(int row = obstacles.GetLength(0) - 1; row >= 0; row--)
        {
            for(int column = 0; column < obstacles.GetLength(1); column++)
            {
                if (row >= obstacles.GetLength(0) - 2 && obstacles[row, column] != null)
                {
                    //To do: shaking.
                    if(obstacles[row, column] is Obstacle)
                    {
                        Obstacle obstacle = (Obstacle)obstacles[row, column];
                        obstacle.Shaking();
                        if (obstacle.Animator.GetBool("isShaking"))
                            countDoneShaking++;
                    }
                    if (row == obstacles.GetLength(0) - 1 && countDoneShaking == countNotNullObstacle && countDoneShaking != 0 && countNotNullObstacle != 0) // Must done shaking.
                    {
                        //To do: gameOver.
                        GameManager.Instance.gameState = GameManager.GameState.gameover;
                    }
                }
            }
        }
    }
}
