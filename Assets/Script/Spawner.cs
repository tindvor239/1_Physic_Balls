﻿using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class Spawner : Singleton<Spawner>
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
    private bool isMoving = true;
    private Obstacle[] warningObstacle = null;
    private Obstacle gameoverObstacle = null;
    public bool spawnOnStart = true;
    private static float maxSpawnRate = 0.7f;
    [SerializeField] [Range(0f, 1f)]
    private float spawnRate = 0.25f;
    private float spawnStartRate;
    [SerializeField] [Range(0f, 1f)]
    private float spawnItemRate = 0.25f;
    #region Singleton
    protected override void OnAwake()
    {
        spawnStartRate = spawnRate;
    }
    #endregion
    #region Properties
    public TwoDimentionalItems Obstacles { get => obstacles; }
    #endregion
    private void Start()
    {
    }
    private void Update()
    {
        // Spawn 4 obstacle on start
        switch(GameManager.Instance.Mode)
        {
            case GameManager.GameMode.survival:
                switch(GameManager.Instance.State)
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
                            if (spawnOnStart)
                            {
                                SpawnOneLine(0.001f);
                            }
                            else
                                SpawnOneLine(0.15f);
                            CheckIsInWarning();
                            GameManager.Instance.isSpawning = false;
                        }
                        break;
                }
                break;
            case GameManager.GameMode.level:
                switch(GameManager.Instance.State)
                {
                    case GameManager.GameState.play:
                        //To do: if list null show win menus.
                        int notNullCount = 0;
                        foreach(Items items in obstacles.rows)
                        {
                            foreach(Item item in items.columns)
                            {
                                if(item != null)
                                {
                                    notNullCount++;
                                }
                            }
                        }
                        if (GameManager.Instance.isSpawning)
                        {
                            //To do: move every blocks up.
                            if(GameManager.Instance.Level.CanMoveUp)
                            {
                                MoveUp(0.15f);
                            }
                            CheckIsInWarning();
                            if (isMoving == false)
                            {
                                OnEndTurn();
                                GetItemBackToPoolOnLastRow();
                                isMoving = true;
                            }

                            GameManager.Instance.isSpawning = false;
                        }
                        if (notNullCount == 0)
                        {
                            GameManager.Instance.State = GameManager.GameState.win;
                        }
                        break;
                }
                break;
        }
    }
    private void SpawnOneLine(float time)
    {
        MoveUp(time);
        if (isMoving == false)
        {
            SpawnItems();
            if (spawnRate >= maxSpawnRate)
                spawnRate = maxSpawnRate;
            else
                spawnRate += 0.02f;
            isMoving = true;
            OnEndTurn();
            minRandomValue = maxRandomValue / 2;
            GetItemBackToPoolOnLastRow();
            maxRandomValue = startMaxRandomValue + (int)GameManager.Instance.turn;
        }
    }
    private void OnEndTurn()
    {
        GameManager.Instance.isEndTurn = true;
        GameManager.Instance.turn++;
        gameoverObstacle = null;
        warningObstacle = null;
        if(GameManager.Instance.Mode == GameManager.GameMode.level)
        {
            GameManager.Instance.SetStars();
        }
    }
    private void MoveUp(float time)
    {
        if(isMoving)
        {
            MoveObstaclesInArrayUp();
            for (int row = 0; row < obstacles.rows.Count; row++)
            {
                for(int column = 0; column < obstacles.rows[0].columns.Count; column++)
                {
                    if (obstacles.rows[row].columns[column] != null)
                    {
                        obstacles.rows[row].columns[column].transform.DOMoveY(obstacles.rows[row].columns[column].transform.position.y + space, time);
                    }
                }
            }
            isMoving = false;
        }
    }
    private List<int> GetSpawnItems()
    {
        List<int> indexes = new List<int>();
        for (byte column = 0; column < obstacles.rows[0].columns.Count; column++)
        {
            //Random that slot is spawn or not.
            float randomSpawn = UnityEngine.Random.Range(0f, 1f);
            if (randomSpawn <= 0.5f)
            {
                indexes.Add(column);
            }
        }
        return indexes;
    }
    private bool CheckCanItSpawn(int column)
    {
        //This method will run first, then it will run random obstacle method.
        if (column < 6 && column > 0)
        {
            if (obstacles.rows[0].columns[column - 1] != null)
            {
                if (obstacles.rows[0].columns[column - 1].transform.position.x > 6.6f - space)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void SpawnItems()
    {
        List<int> indexes = GetSpawnItems();
        for(int index = 0; index < indexes.Count; index++)
        {
            if(CheckCanItSpawn(indexes[index]))
            {
                float spawnRoll = UnityEngine.Random.Range(0f, 1f);
                if(spawnRoll <= spawnRate)
                {
                    SpawnItem(GameManager.Instance.PoolParty.GetPool("Obstacles Pool"), indexes[index]);
                }
                else
                {
                    if(CheckIsRowHaveItem() == false)
                    {
                        float itemSpawnRoll = UnityEngine.Random.Range(0f, 1f);
                        if(itemSpawnRoll <= spawnItemRate)
                            SpawnItem(GameManager.Instance.PoolParty.GetPool("Items Pool"), indexes[index]);
                    }
                }
            }
        }
        if(CheckIsRowEmpty())
        {
            SpawnItems();
        }
    }
    private bool CheckIsRowEmpty()
    {
        foreach(Item item in obstacles.rows[0].columns)
        {
            if(item != null)
            {
                return false;
            }
        }
        return true;
    }
    private Vector2 GetRandomPos(int index)
    {
        Vector2 randomPos = new Vector2(transform.position.x + (space * index), transform.position.y);
        return randomPos;
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
    private void SpawnItem(Pool pool, int column)
    {
        bool isGotIt = false;
        int randomItem = UnityEngine.Random.Range(0, pool.ObjectsToPool.Length);
        GameObject newGameObject = null;
        if (pool.CanExtend)
        {
            newGameObject = GameManager.Instance.PoolParty.CreateItem(pool, new Vector2(transform.position.x + startPosition + (space * column), transform.position.y), randomItem, transform);

        }
        else
        {
            foreach (GameObject gameObject in pool.ObjectsPool)
            {
                if(gameObject.activeInHierarchy == false && isGotIt == false)
                {
                    if(gameObject.GetComponent<Obstacle>() != null)
                    {
                        Obstacle obstacle = gameObject.GetComponent<Obstacle>();
                        if(obstacle.Geometry == pool.ObjectsToPool[randomItem].GetComponent<Obstacle>().Geometry)
                        {
                            newGameObject = pool.GetOutOfPool(gameObject, GetRandomPos(column));
                            isGotIt = true;
                        }
                    }
                    else if(randomItem == 0)
                    {
                        if(gameObject.GetComponent<SizeItem>() != null)
                        {
                            newGameObject = pool.GetOutOfPool(gameObject, GetRandomPos(column));
                            isGotIt = true;
                        }
                    }
                    else if(randomItem == 1)
                    {
                        if(gameObject.GetComponent<AddItem>() != null)
                        {
                            newGameObject = pool.GetOutOfPool(gameObject, GetRandomPos(column));
                            isGotIt = true;
                        }
                    }
                }
            }
        }
        if(newGameObject != null)
        {
            if(GameManager.Instance.Mode == GameManager.GameMode.survival)
            {
                newGameObject.transform.localScale = pool.ObjectsToPool[randomItem].transform.localScale;
                Debug.Log("1st size" + newGameObject.transform.localScale.x + ", 2nd size" + pool.ObjectsToPool[randomItem].transform.localScale.x);
            }
            if(newGameObject.GetComponent<Obstacle>())
            {
                Obstacle obstacle = newGameObject.GetComponent<Obstacle>();
                obstacle.HP = UnityEngine.Random.Range(minRandomValue, maxRandomValue);
                GameManager.Instance.SetSpriteColor(obstacle);
            }
            obstacles.rows[0].columns[column] = newGameObject.GetComponent<Item>();
        }
    }
    private void MoveObstaclesInArrayUp()
    {
        for(int row = obstacles.rows.Count - 1; row >= 0; row--)
        {
            for(int column = 0; column < obstacles.rows[0].columns.Count; column++)
            {
                if (obstacles.rows[row].columns[column] != null && row < obstacles.rows.Count - 1)
                {
                    if(row + 1 <= obstacles.rows.Count && obstacles.rows[row + 1].columns[column] == null)
                    {
                        obstacles.rows[row + 1].columns[column] = obstacles.rows[row].columns[column];
                        obstacles.rows[row].columns[column] = null;
                    }
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
    private bool CheckIsRowHaveItem()
    {
        foreach(Item item in obstacles.rows[0].columns)
        {
            if(item is AddItem || item is SizeItem)
            {
                return true;
            }
        }
        return false;
    }
}
