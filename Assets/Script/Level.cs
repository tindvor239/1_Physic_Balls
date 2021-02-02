using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField]
    private string name;
    [SerializeField]
    private byte stars;
    [SerializeField]
    private int row, column;
    [SerializeField]
    private byte[] points;
    [SerializeField]
    private byte turnCount;
    [SerializeField]
    private bool canMoveUp;
    [SerializeField]
    private List<Ball> balls;
    [SerializeField]
    private List<Item> items;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private StorageJson storage = new StorageJson();
    #region Properties
    public string Name { get => name; set => name = value; }
    public byte Stars { get => stars; set => stars = value; }
    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
    public List<Ball> Balls { get => balls; set => balls = value; }
    public List<Item> Items { get => items; set => items = value; }
    public StorageJson Storage { get => storage; }
    public GameObject BallPrefab { get => ballPrefab; }
    public byte[] Points { get => points; set => points = value; }
    public byte TurnCount { get => turnCount; set => turnCount = value; }
    public bool CanMoveUp { get => canMoveUp; set => canMoveUp = value; }
    #endregion

    public void Load(LevelPackage levelPackage)
    {
        storage.ConvertedLevel = levelPackage;
        storage.ConvertedLevel.Unpack(this);
        Items[] items = null;
        Item[] contentItems = null;
        if (GameManager.Instance.Mode == GameManager.GameMode.editor)
        {
            MapEditor.Instance.Row = (uint)(row - 1);
            MapEditor.Instance.Column = (uint)(column - 1);
            items = new Items[row];
            contentItems = new Item[column];
        }
        else
        {
            items = new Items[row];
            contentItems = new Item[column];
        }
        Spawner.Instance.Obstacles.rows = items.ToList();
        for(int index = 0; index < items.Length; index++)
        {
            Spawner.Instance.Obstacles.rows[index] = new Items();
            Spawner.Instance.Obstacles.rows[index].Name = string.Format("row {0}", index);
            Spawner.Instance.Obstacles.rows[index].columns = contentItems.ToList();
        }
        SpawnBalls(levelPackage);
        SpawnItems(levelPackage.Items);
    }
    public void LoadSurvival(LevelPackage levelPackage)
    {
        string levelString = "";
        PlayerPrefs.GetString("survival", levelString);
    }
    private void SpawnBalls(LevelPackage levelPackage)
    {
        for (int count = 0; count < levelPackage.Balls; count++)
        {
            //To do: spawn ball.
            Ball ball = GameManager.Instance.CreateObject(GameManager.Instance.GameScene.transform, GameManager.Instance.Level.BallPrefab).GetComponent<Ball>();
            ball.transform.position = GameManager.Instance.SpawnBall.transform.position;
            //GameManager.Instance.Level.Balls.Add(ball);
            Shooter.Instance.Balls.Add(ball);
        }
        Shooter.Instance.Reload();
    }
    private void SpawnItems(List<string> itemsString)
    {
        PoolParty poolParty = GameManager.Instance.PoolParty;
        int row = 0;
        int column = 0;
        foreach (string itemString in itemsString)
        {
            string type = "";
            int prefabIndex = 0;
            //if the package is obstacle
            if (itemString.IndexOf("\"type\":\"Obstacle\"") != -1)
            {
                Debug.Log("we have a obstacle");
                type = "Obstacles Pool";
                CreaturePackage package = new CreaturePackage();
                package = (CreaturePackage)JsonUtility.FromJson(itemString, typeof(CreaturePackage));
                for (int index = 0; index < poolParty.GetPool(type).ObjectsToPool.Length; index++)
                {
                    if (poolParty.GetPool(type).ObjectsToPool[index].GetComponent<Obstacle>().Geometry.ToString() == package.geometry)
                    {
                        prefabIndex = index;
                    }
                }
                GameObject item = CreateOrGetObstacle(poolParty, poolParty.GetPool(type), prefabIndex);
                GameObject text = item.GetComponentInChildren<RectTransform>().gameObject;
                Spawner.Instance.Obstacles.rows[row].columns[column] = item.GetComponent<Obstacle>();
                package.Unpack(item);
                GameManager.Instance.SetSpriteColor(item.GetComponent<Obstacle>());
            }
            else if(itemString.IndexOf("\"type\":\"DeadItem\"") != -1)
            {
                Debug.Log("we have a dead item");
                type = "Dead Obstacles Pool";
                DeadPackage package = new DeadPackage();
                package = (DeadPackage)JsonUtility.FromJson(itemString, typeof(DeadPackage));
                for(int index = 0; index < poolParty.GetPool(type).ObjectsToPool.Length; index++)
                {
                    if(poolParty.GetPool(type).ObjectsToPool[index].GetComponent<DeadItem>().Geometry.ToString() == package.geometry)
                    {
                        prefabIndex = index;
                    }
                }
                GameObject item = CreateOrGetObstacle(poolParty, poolParty.GetPool(type), prefabIndex);
                Spawner.Instance.Obstacles.rows[row].columns[column] = item.GetComponent<Obstacle>();
                package.Unpack(item);
            }
            else if (itemString.IndexOf("\"type\":\"AddItem\"") != -1 || itemString.IndexOf("\"type\":\"SizeItem\"") != -1)
            {
                type = "Items Pool";
                Package package = new Package();
                package = (Package)JsonUtility.FromJson(itemString, typeof(Package));
                for (int index = 0; index < poolParty.GetPool(type).ObjectsToPool.Length; index++)
                {
                    if (package.type == "AddItem" && poolParty.GetPool(type).ObjectsToPool[index].GetComponent<AddItem>())
                        prefabIndex = index;
                    else if (package.type == "SizeItem" && poolParty.GetPool(type).ObjectsToPool[index].GetComponent<SizeItem>())
                        prefabIndex = index;
                }
                GameObject item = CreateOrGetItem(poolParty, poolParty.GetPool(type),prefabIndex);
                Spawner.Instance.Obstacles.rows[row].columns[column] = item.GetComponent<Item>();
                package.Unpack(item);
            }
            column++;
            if (column >= GameManager.Instance.Level.Column)
            {
                column = 0;
                row++;
            }
        }
    }
    private GameObject CreateOrGetObstacle(PoolParty poolParty, Pool pool, int prefabIndex)
    {
        bool isGotIt = false;
        GameObject newGameObject = null;
        foreach(GameObject gameObject in pool.ObjectsPool)
        {
            if(gameObject != null && gameObject.activeInHierarchy == false &&
               gameObject.GetComponent<Obstacle>().Geometry == pool.ObjectsToPool[prefabIndex].GetComponent<Obstacle>().Geometry && isGotIt == false)
            {
                newGameObject = pool.GetOutOfPool(gameObject, GameManager.Instance.transform.position);
                isGotIt = true;
                if (newGameObject == null)
                {
                    Debug.Log("Get out of pool but still null");
                }
            }
        }
        if(isGotIt == false)
        {
            newGameObject = poolParty.CreateItem(pool, GameManager.Instance.transform.position, prefabIndex, Spawner.Instance.transform);
            if(newGameObject == null)
            {
                Debug.Log("Create but still null");
            }
        }
        return newGameObject;
    }
    private GameObject CreateOrGetItem(PoolParty poolParty, Pool pool, int prefabIndex)
    {
        bool isGotIt = false;
        GameObject newGameObject = null;
        if (IsItemExistInBool(pool, prefabIndex) == false)
        {
            newGameObject = poolParty.CreateItem(pool, GameManager.Instance.transform.position, prefabIndex, Spawner.Instance.transform);
        }
        else
        {
            foreach (GameObject gameObject in pool.ObjectsPool)
            {
                if (gameObject.activeInHierarchy == false && isGotIt == false)
                {
                    if(prefabIndex == 0)
                    {
                        if (gameObject.GetComponent<SizeItem>() != null)
                        {
                            newGameObject = pool.GetOutOfPool(gameObject, GameManager.Instance.transform.position);
                            isGotIt = true;
                        }
                    }
                    else if(prefabIndex == 1)
                    {
                        if(gameObject.GetComponent<AddItem>() != null)
                        {
                            newGameObject = pool.GetOutOfPool(gameObject, GameManager.Instance.transform.position);
                        }
                    }
                }
            }
        }
        return newGameObject;
    }
    //This method only use for ITEMS POOL;
    private bool IsItemExistInBool(Pool pool, int prefabIndex)
    {
        if(prefabIndex == 0)
        {
            foreach(GameObject go in pool.ObjectsPool)
            {
                if(go.GetComponent<SizeItem>() != null && go.activeInHierarchy == false)
                {
                    return true;
                }
            }
        }
        else if(prefabIndex == 1)
        {
            foreach(GameObject go in pool.ObjectsPool)
            {
                if(go.GetComponent<AddItem>() != null && go.activeInHierarchy == false)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void UpdateStatus()
    {
        int levelTurn = turnCount - ((int)GameManager.Instance.turn - 1);
        if (levelTurn > points[2])
        {
            stars = 3;
        }
        else if (levelTurn <= points[2])
        {
            stars = 2;
        }
        if (levelTurn <= points[1])
        {
            stars = 1;
        }
        if (levelTurn <= points[0])
        {
            stars = 0;
            GameManager.Instance.State = GameManager.GameState.gameover;
        }
    }
}
