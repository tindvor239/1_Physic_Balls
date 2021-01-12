using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private uint row, column;
    private uint lastRow, lastColumn;
    [SerializeField]
    private Vector2 offset, size;
    private Vector2 lastSpace, lastSize;
    #region Properties
    public GameObject GridPrefab { get => gridPrefab; }
    public GameObject GridIndexerPrefab { get => gridIndexerPrefab; }
    public List<GameObject> GridRowIndexeres { get => gridRowIndexeres; }
    public Spawner Spawner { get => spawner; }
    public uint Row { get => row; }
    public uint Column { get => column; }
    public Vector2 Space { get => offset; }
    #endregion
    // Update is called once per frame
    private void Start()
    {
        spawner = Spawner.Instance;
        row = (uint)Spawner.Instance.Obstacles.rows.Count;
        column = (uint)Spawner.Instance.Obstacles.rows[0].columns.Count;
    }
    private void Update()
    {
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
        RaycastHit2D hit;
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "Editor")
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
                        if(brush == pool.ObjectsToPool[i])
                        {
                            int[] tileIndex = GetTileIndex(hit.collider.gameObject);
                            if(tileIndex[0] != -1)
                            {
                                SpawnItemFromPool(1, pool, hit.collider.gameObject.transform.position, Spawner.Instance.Obstacles.rows[tileIndex[0]].columns, tileIndex[1], i);
                            }
                        }
                    }
                }
                Debug.Log(hit.collider.gameObject.name);
            }
        }
        if(row != lastRow || column != lastColumn || size != lastSize || offset != lastSpace)
        {
            // reupdate if anything is change.
            if(row != lastRow)
            {
                lastRow = row;
            }
            if(column != lastColumn)
            {
                lastColumn = column;
            }
            if (size != lastSize)
            {
                lastSize = size;
            }
            if (offset != lastSpace)
            {
                lastSpace = offset;
            }
            SpawnGridIndexeres();
            SpawnGridTiles();
            Items[] items = new Items[row];
            foreach(Items _items in items)
            {
                //Null exception error, But recheck back the class Level (when load renew the column and row of spawner).
                Item[] items_ = new Item[column];
                _items.columns = items_.ToList();
            }
            Spawner.Instance.Obstacles.rows = items.ToList();
        }
    }
    private int[] GetTileIndex(GameObject selectedObject)
    {
        int row = 0, column = 0;
        int[] result = {-1, -1};
        for(int i = 0; i < gridTiles.Count; i++)
        {
            column++;
            if(column >= this.column)
            {
                column = 0;
                row++;
            }
            if(gameObject == selectedObject)
            {
                result[0] = row;
                result[1] = column;
                return result;
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
                    brush = gameObject;
                }
            }
        }
    }
    private void SpawnGridTiles()
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Tile Prefab");
        DestroyExistInList(pool, gridTiles);
        for (int row = 0; row <= this.row; row++)
        {
            for(int column = 0; column <= this.column; column++)
            {
                float x = gridColumnIndexeres[column].transform.position.x;
                float y = gridRowIndexeres[row].transform.position.y;
                Vector2 position = new Vector2(x, y);
                SpawnPrefabFromPool(0, pool, position, gridTiles);
            }
        }
    }
    private void SpawnGridIndexeres()
    {
        Pool pool = GameManager.Instance.PoolParty.GetPool("Tile Indexeres Pool");
        DestroyExistInList(pool, gridRowIndexeres);
        for (int row = 0; row <= this.row; row++)
        {
            SpawnPrefabFromPool(row, pool, new Vector2(Spawner.Instance.transform.position.x - offset.x, Spawner.Instance.transform.position.y + (offset.y * row)), gridRowIndexeres);
        }
        DestroyExistInList(pool, gridColumnIndexeres);
        for (int column = 0; column <= this.column; column++)
        {
            SpawnPrefabFromPool(column, pool, new Vector2(Spawner.Instance.transform.position.x + (offset.y * column), Spawner.Instance.transform.position.y - offset.x), gridColumnIndexeres);
        }
    }
    private void SpawnPrefabFromPool(int number, Pool pool, Vector2 position, List<GameObject> gameObjects, int objectFromPoolIndex = 0)
    {
        GameObject newGameObject = SpawnItem(pool, position, objectFromPoolIndex);
        newGameObject.transform.localScale = size;
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
        newGameObject.transform.localScale = size;
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
