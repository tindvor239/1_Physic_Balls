using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MapEditor : Singleton<MapEditor>
{
    [SerializeField]
    private GameObject gridPrefab, gridIndexerPrefab;
    [SerializeField] //just for show.
    private List<GameObject> gridRowIndexeres = new List<GameObject>();
    [SerializeField] //just for show.
    private List<GameObject> gridColumnIndexeres = new List<GameObject>();
    [SerializeField] //just for show.
    private List<GameObject> gridTiles = new List<GameObject>();
    [SerializeField] //just for show.
    private GameObject brush;
    [SerializeField]
    private Spawner spawner;
    [SerializeField]
    private InputField row, column, space, sizeX, sizeY;
    [SerializeField]
    private bool isBeta = false;
    private int lastRow = -1, lastColumn = -1;
    private float indexerSpace = 2;
    private Vector2 lastSize;
    private float lastSpace;
    #region Properties
    public GameObject GridPrefab { get => gridPrefab; }
    public GameObject GridIndexerPrefab { get => gridIndexerPrefab; }
    public List<GameObject> GridRowIndexeres { get => gridRowIndexeres; }
    public Spawner Spawner { get => spawner; }
    public uint Row
    {
        get
        {
            try
            {
                return uint.Parse(row.text);
            }
            catch(Exception)
            {
                if(row != null)
                {
                    row.text = "0";
                }
                return 0;
            }
        }
        set
        {
            row.text = value.ToString();
        }
    }
    public uint Column
    {
        get
        {
            try
            {
                return uint.Parse(column.text);
            }
            catch(Exception)
            {
                if(column != null)
                {
                    column.text = "0";
                }
                return 0;
            }
        }
        set
        {
            column.text = value.ToString();
        }
    }
    public float Space
    {
        get
        {
            try
            {
                return float.Parse(space.text);
            }
            catch(Exception)
            {
                if(space != null)
                {
                    space.text = "1";
                }
                return 1;
            }
        }
        set
        {
            space.text = value.ToString();
        }
    }
    public Vector2 Size
    {
        get
        {
            float x, y;
            try
            {
                x = float.Parse(sizeX.text);
            }
            catch(Exception)
            {
                if(sizeX != null)
                {
                    sizeX.text = "1";
                }
                x = 1;
            }
            try
            {
                y = float.Parse(sizeY.text);
            }
            catch(Exception)
            {
                if(sizeY.text != null)
                {
                    sizeY.text = "1";
                }
                y = 1;
            }
            return new Vector2(x, y);
        }
    }
    #endregion
    // Update is called once per frame
    private void Start()
    {
        spawner = Spawner.Instance;
    }
    private void Update()
    {
        if(isBeta)
        {
            if(Row != lastRow || Column != lastColumn || Size != lastSize || Space != lastSpace)
        {
            // reupdate if anything is change.
            if (Row != lastRow)
            {
                lastRow = (int)Row;
            }
            if (Column != lastColumn)
            {
                lastColumn = (int)Column;
            }
            if (Size != lastSize)
            {
                lastSize = Size;
            }
            if (Space != lastSpace)
            {
                lastSpace = Space;
            }
            SpawnGridIndexeres();
            SpawnGridTiles();
            resizeObstacles();
        }
            if(Input.GetMouseButton(0))
            {
                GetTileObject();
            }
        }
        string path = string.Format("{0}/{1}/{2}.json", Application.dataPath + "/Resources/", GameManager.Instance.Level.Storage.FolderName, "level_" + GameManager.Instance.Level.Name);
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.Level.Storage.Save(GameManager.Instance.Level);
            GameManager.Instance.Level.Storage.ConvertJsonToObject(path);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.Level.Load(GameManager.Instance.Level.Storage.ConvertedLevel);
        }
    }

    private void GetTileObject()
    {
        RaycastHit2D hit;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Editor")
            {
                //spawn obstacle here.
                Pool pool = null;
                if (brush.GetComponent<Item>() is Obstacle)
                {
                    pool = GameManager.Instance.PoolParty.GetPool("Obstacles Pool");
                }
                //else if item
                for (int i = 0; i < pool.ObjectsToPool.Length; i++)
                {
                    if (brush == pool.ObjectsToPool[i])
                    {
                        int[] tileIndex = GetTileIndex(hit.collider.gameObject);
                        if (tileIndex[0] != -1)
                        {
                            SpawnItemFromPool(1, pool, hit.collider.gameObject.transform.position, Spawner.Instance.Obstacles.rows[tileIndex[0]].columns, tileIndex[1], i);
                        }
                    }
                }
            }
        }
    }

    private void resizeObstacles()
    {
        Items[] items = new Items[Row + 1];
        List<int[]> existIndexes = CheckExistObstacleInRange();
        for (int index = 0; index < items.Length; index++)
        {
            items[index] = new Items();
            items[index].Name = string.Format("row {0}", index);
            Item[] items_ = new Item[Column + 1];
            items[index].columns = items_.ToList();
        }
        if(existIndexes.Count != 0)
        {
            foreach(int[] index in existIndexes)
            {
                items[index[0]].columns[index[1]] = Spawner.Instance.Obstacles.rows[index[0]].columns[index[1]];
            }
        }
        Spawner.Instance.Obstacles.rows = items.ToList();
    }
    private List<int[]> CheckExistObstacleInRange()
    {
        List<int[]> indexes = new List<int[]>();
        for(int row = 0; row < Spawner.Instance.Obstacles.rows.Count; row++)
        {
            for(int column = 0; column < Spawner.Instance.Obstacles.rows[row].columns.Count; column++)
            {
                if(Spawner.Instance.Obstacles.rows[row].columns[column] != null)
                {
                    //If existed obstacle is out of range will be move back to pool.
                    if (row < Row && column < Column)
                    {
                        int[] index = new int[2];
                        index[0] = row;
                        index[1] = column;
                        indexes.Add(index);
                    }
                    else
                    {
                        if (Spawner.Instance.Obstacles.rows[row].columns[column].GetComponent<Item>() is Obstacle)
                        {
                            Pool pool = GameManager.Instance.PoolParty.GetPool("Obstacles Pool");
                            pool.GetBackToPool(Spawner.Instance.Obstacles.rows[row].columns[column].gameObject, GameManager.Instance.transform.position);
                        }
                    }
                }
            }
        }
        return indexes;
    }
    private int[] GetTileIndex(GameObject selectedObject)
    {
        int row = 0, column = 0;
        int[] result = {-1, -1};
        for(int i = 0; i < gridTiles.Count; i++)
        {
            if(gridTiles[i] == selectedObject && Spawner.Instance.Obstacles.rows[row].columns[column] == null)
            {
                result[0] = row;
                result[1] = column;
                return result;
            }
            if(column <= Column)
            {
                column++;
            }
            if(column > Column)
            {
                column = 0;
                if(row < Row)
                {
                    row++;
                }
            }
        }
        return result;
    }
    public void SetBrush(Sprite sprite)
    {
        foreach (GameObject gameObject in GameManager.Instance.PoolParty.GetPool("Obstacles Pool").ObjectsToPool)
        {
            if(gameObject.GetComponent<SpriteRenderer>())
            {
                if(sprite == gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite.name);
                    brush = gameObject;
                }
            }
        }
    }
    private void SpawnGridTiles()
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Tile Prefab");
        DestroyExistInList(pool, gridTiles);
        List<int[]> existedIndex = CheckExistObstacleInRange();
        for (int row = 0; row <= Row; row++)
        {
            for(int column = 0; column <= Column; column++)
            {
                float x = gridColumnIndexeres[column].transform.position.x;
                float y = gridRowIndexeres[row].transform.position.y;
                Vector2 position = new Vector2(x, y);
                foreach(int[] index in existedIndex)
                {
                    if(index[0] == row && index[1] == column)
                    {
                        Spawner.Instance.Obstacles.rows[index[0]].columns[index[1]].transform.position = position;
                    }
                }
                SpawnPrefabFromPool(0, pool, position, gridTiles, 0, row, column);
            }
        }
    }
    private void SpawnGridIndexeres()
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Tile Indexeres Pool");
        DestroyExistInList(pool, gridRowIndexeres);
        for (int row = 0; row <= Row; row++)
        {
            SpawnPrefabFromPool(row, pool, new Vector2(Spawner.Instance.transform.position.x -indexerSpace, Spawner.Instance.transform.position.y + (Space * row)), gridRowIndexeres);
        }
        DestroyExistInList(pool, gridColumnIndexeres);
        for (int column = 0; column <= Column; column++)
        {
            SpawnPrefabFromPool(column, pool, new Vector2(Spawner.Instance.transform.position.x + (Space * column), Spawner.Instance.transform.position.y - indexerSpace), gridColumnIndexeres);
        }
    }
    private void SpawnPrefabFromPool(int number, Pool pool, Vector2 position, List<GameObject> gameObjects, int objectFromPoolIndex = 0, int row = -1, int column = -1)
    {
        GameObject newGameObject = SpawnItem(pool, position, objectFromPoolIndex);
        if(row != -1 && column != -1)
        {
            newGameObject.name = string.Format("{0}-{1}_{2}", gridPrefab.name, row, column);
        }
        newGameObject.transform.localScale = Size;
        if(newGameObject.GetComponent<UIMenu>())
        {
            UIMenu gridIndexerMenu = newGameObject.GetComponent<UIMenu>();
            gridIndexerMenu.MenuInfos[0].text = number.ToString();
        }
        gameObjects.Add(newGameObject);
    }
    private void SpawnItemFromPool(int number, Pool pool, Vector2 position, List<Item> items, int itemsIndex, int objectFromPoolIndex = 0)
    {
        GameObject newGameObject = SpawnItem(pool, position, objectFromPoolIndex);
        Item item = newGameObject.GetComponent<Item>();
        newGameObject.transform.localScale = Size;
        if (item is Obstacle)
        {
            Obstacle obstacle = (Obstacle)item;
            obstacle.HP = number;
        }
        items[itemsIndex] = item;
    }
    private void DestroyExistInList(Pool pool, List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null)
            {
                pool.GetBackToPool(gameObject, GameManager.Instance.transform.position);
            }
        }
        gameObjects.RemoveAll(gameObject => gameObject != null);
        gameObjects.RemoveAll(gameObject => gameObject == null);
    }
    private GameObject SpawnItem(Pool pool, Vector2 position, int index)
    {
        bool isGotIt = false;
        GameObject newGameObject = null;
        if (pool.CanExtend)
        {
            newGameObject = GameManager.Instance.PoolParty.CreateItem(pool, position, index, transform);
        }
        else
        {
            foreach (GameObject gameObject in pool.ObjectsPool)
            {
                if (gameObject.activeInHierarchy == false && isGotIt == false)
                {
                    newGameObject = pool.GetOutOfPool(gameObject, position);
                    isGotIt = true;
                }
            }
        }
        return newGameObject;
    }
}
