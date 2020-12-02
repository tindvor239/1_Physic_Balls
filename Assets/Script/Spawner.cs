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
    private byte spawnCount;
    [SerializeField]
    private GameObject[] obstaclePrefabs;
    private static uint startMaxRandomValue = 1;
    [SerializeField]
    private uint maxRandomValue = startMaxRandomValue;
    [SerializeField]
    private uint minRandomValue = 1;
    private Item[,] obstacles = new Item[9, 6];
    private byte countSpawnOnStart;
    private byte countDoneMoving = 0, countDoneShaking = 0, countInWarning = 0, countNotNullObstacle = 0;
    private bool isDoneMoveUpInArray = false, isDoneMoving = false;
    private bool isInWarning = false, isDoneChecking = false;
    private bool spawnOnStart = true;
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
            GameManager.Instance.isSpawning = false;
            GameManager.Instance.isEndTurn = true;
            GameManager.Instance.turn++;
            if (maxRandomValue <= 3)
                minRandomValue = maxRandomValue;
            else
                minRandomValue = maxRandomValue - (GameManager.Instance.turn * 2);
            maxRandomValue = startMaxRandomValue + (GameManager.Instance.turn * 5);
            isDoneMoving = false;
        }
    }
    private void SpawnItems()
    {
        for(byte column = 0; column < obstacles.GetLength(1); column++)
        {
            //Random that slot is spawn or not.
            int randomSpawn = Random.Range(0, (int)GameManager.Instance.turn + 2);
            if(randomSpawn >= 1)
            {
                //if 1 is circle, 0 is square.
                if (spawnCount != 4)
                    SpawnObstacle(column);
                else
                {
                    SpawnItem(column);
                    spawnCount = 0;
                }
            }
        }
        spawnCount++;
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
                    countInWarning++;
                }
            }
        }
        if (countInWarning > 0)
            return true;
        else return false;
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
