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
    [SerializeField] //just for show.
    private GameObject selectedObject;
    [SerializeField]
    private Image brushImage;
    [SerializeField]
    private Spawner spawner;
    [SerializeField]
    private InputField row, column, space, sizeX, sizeY, privatePositionX, privatePositionY, privateSizeX, privateSizeY, rotationZ, hp;
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
    private Vector2 PrivateSize
    {
        get
        {
            float x, y;
            try
            {
                x = float.Parse(privateSizeX.text);
            }
            catch (Exception)
            {
                if (privateSizeX != null)
                {
                    privateSizeX.text = "1";
                }
                x = 1;
            }
            try
            {
                y = float.Parse(privateSizeY.text);
            }
            catch (Exception)
            {
                if (privateSizeY.text != null)
                {
                    privateSizeY.text = "1";
                }
                y = 1;
            }
            return new Vector2(x, y);
        }
        set
        {
            privateSizeX.text = value.x.ToString();
            privateSizeY.text = value.y.ToString();
        }
    }
    private Vector2 PrivatePosition
    {
        get
        {
            float x, y;
            try
            {
                x = float.Parse(privatePositionX.text);
            }
            catch (Exception)
            {
                if (privatePositionX != null)
                {
                    privatePositionX.text = "1";
                }
                x = 1;
            }
            try
            {
                y = float.Parse(privatePositionY.text);
            }
            catch (Exception)
            {
                if (privatePositionY.text != null)
                {
                    privatePositionY.text = "1";
                }
                y = 1;
            }
            return new Vector2(x, y);
        }
        set
        {
            privatePositionY.text = value.x.ToString();
            privatePositionY.text = value.y.ToString();
        }
    }
    private float Rotation
    {
        get
        {
            try
            {
                return float.Parse(rotationZ.text);
            }
            catch (Exception)
            {
                if (rotationZ != null)
                {
                    rotationZ.text = "0";
                }
                return 1;
            }
        }
        set
        {
            rotationZ.text = value.ToString();
        }
    }
    private int HP
    {
        get
        {
            try
            {
                return int.Parse(hp.text);
            }
            catch (Exception)
            {
                if (hp != null)
                {
                    hp.text = "1";
                }
                return 1;
            }
        }
        set
        {
            hp.text = value.ToString();
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
                // Update if anything is change.
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
            // Update if anything is change.
            if(selectedObject != null)
            {
                bool isObstacle = selectedObject.GetComponent<Obstacle>() && HP != selectedObject.GetComponent<Obstacle>().HP;
                if (PrivateSize != (Vector2)selectedObject.transform.localScale || PrivatePosition != (Vector2)selectedObject.transform.position
                    || Rotation != selectedObject.transform.rotation.z || isObstacle)
                {
                    if(PrivatePosition != (Vector2)selectedObject.transform.position)
                    {
                        selectedObject.transform.position = PrivatePosition;
                    }
                    if(PrivateSize != (Vector2)selectedObject.transform.localScale)
                    {
                        selectedObject.transform.localScale = PrivateSize;
                    }
                }
            }
            // Get Object On Click
            if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                RaycastHit2D hit;
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if(Input.GetMouseButton(0))
                {
                    GetTileObject(hit);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    //Spawn Obstacle Menu.
                    ShowObstacleMenu(hit);
                }
            }
        }
        string path = string.Format("{0}/{1}/{2}.json", Application.dataPath + "/Resources/", GameManager.Instance.Level.Storage.FolderName, "level_" + GameManager.Instance.Level.Name);
        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach(Items items in Spawner.Instance.Obstacles.rows)
            {
                items.columns.RemoveAll(item => item == null);
            }
            GameManager.Instance.Level.Storage.Save(GameManager.Instance.Level);
            GameManager.Instance.Level.Storage.ConvertJsonToObject(path);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.Level.Load(GameManager.Instance.Level.Storage.ConvertedLevel);
        }
    }

    private void GetTileObject(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Editor" && brush != null)
            {
                //spawn obstacle here.
                Painting(hit);
            }
            else if (hit.collider.gameObject.tag == "Obstacle" && brush == null)
            {
                DestroySelectedObject(hit);
            }
        }
    }
    private void Painting(RaycastHit2D hit)
    {
        Pool pool = null;
        if (brush.GetComponent<Item>() is Obstacle)
        {
            pool = GameManager.Instance.PoolParty.GetPool("Obstacles Pool");
        }
        //else if item
        else if (brush.GetComponent<Item>() is SizeItem || brush.GetComponent<Item>() is AddItem)
        {
            pool = GameManager.Instance.PoolParty.GetPool("Items Pool");
        }
        else if(brush.GetComponent<Item>() is DeadItem)
        {
            pool = GameManager.Instance.PoolParty.GetPool("Dead Obstacles Pool");
        }
        for (int i = 0; i < pool.ObjectsToPool.Length; i++)
        {
            int[] tileIndex = GetTileIndex(hit.collider.gameObject);
            if (brush.GetComponent<Obstacle>() != null && brush == pool.ObjectsToPool[i])
            {
                if (tileIndex[0] != -1)
                {
                    SpawnItemFromPool(1, pool, hit.collider.gameObject.transform.position, Spawner.Instance.Obstacles.rows[tileIndex[0]].columns, tileIndex[1], i);
                }
            }
            // if brush is item
            else if (brush.GetComponent<SizeItem>() != null && brush == pool.ObjectsToPool[i] && pool.ObjectsToPool[i].GetComponent<SizeItem>())
            {
                if (tileIndex[0] != -1)
                {
                    SpawnItemFromPool(0, pool, hit.collider.transform.position, Spawner.Instance.Obstacles.rows[tileIndex[0]].columns, tileIndex[1], i);
                }
            }
            else if (brush.GetComponent<AddItem>() != null && brush == pool.ObjectsToPool[i] && pool.ObjectsToPool[i].GetComponent<AddItem>())
            {
                if (tileIndex[0] != -1)
                {
                    SpawnItemFromPool(0, pool, hit.collider.transform.position, Spawner.Instance.Obstacles.rows[tileIndex[0]].columns, tileIndex[1], i);
                }
            }
        }
    }
    private void ShowObstacleMenu(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.tag == "Obstacle")
        {
            DisplayObstacleMenu(true);
            GetObstacle(hit);
        }
    }
    private void GetObstacle(RaycastHit2D hit)
    {
        if(hit.collider.gameObject.tag == "Obstacle")
        {
            selectedObject = hit.collider.gameObject;
            PrivatePosition = selectedObject.transform.position;
            PrivateSize = selectedObject.transform.localScale;
            Rotation = selectedObject.transform.rotation.z;
            HP = selectedObject.GetComponent<Obstacle>().HP;
        }
    }
    private void DestroySelectedObject(RaycastHit2D hit)
    {
        if(hit.collider.gameObject.tag == "Obstacle")
        {
            if(hit.collider.gameObject.GetComponent<AddItem>() || hit.collider.gameObject.GetComponent<SizeItem>())
            {
                GameManager.Instance.PoolParty.GetPool("Items Pool").GetBackToPool(hit.collider.gameObject, GameManager.Instance.transform.position);
            }
            else if(hit.collider.gameObject.GetComponent<Obstacle>())
            {
                GameManager.Instance.PoolParty.GetPool("Obstacles Pool").GetBackToPool(hit.collider.gameObject, GameManager.Instance.transform.position);
            }
            else if(hit.collider.gameObject.GetComponent<DeadItem>())
            {
                GameManager.Instance.PoolParty.GetPool("Dead Obstacles Pool").GetBackToPool(selectedObject, GameManager.Instance.transform.position);
            }
            foreach(Items items in Spawner.Instance.Obstacles.rows)
            {
                for(int column = 0; column < items.columns.Count; column++)
                {
                    if(items.columns[column] != null && items.columns[column].gameObject == hit.collider.gameObject)
                    {
                        items.columns[column] = null;
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
    public void SetObstacleBrush(Sprite sprite)
    {
        foreach (GameObject gameObject in GameManager.Instance.PoolParty.GetPool("Obstacles Pool").ObjectsToPool)
        {
            if(gameObject.GetComponent<Obstacle>().MainImage.GetComponent<SpriteRenderer>())
            {
                if(sprite == gameObject.GetComponent<Obstacle>().MainImage.GetComponent<SpriteRenderer>().sprite)
                {
                    Debug.Log(gameObject.GetComponent<Obstacle>().MainImage.GetComponent<SpriteRenderer>().sprite.name);
                    brush = gameObject;
                }  
            }
        }
    }
    public void EraseBrush()
    {
        brush = null;
    }
    public void SetDeadObstacleBrush(Sprite sprite)
    {
        foreach (GameObject gameObject in GameManager.Instance.PoolParty.GetPool("Dead Obstacles Pool").ObjectsToPool)
        {
            if (gameObject.GetComponent<SpriteRenderer>())
            {
                if (sprite == gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite.name);
                    brush = gameObject;
                }
            }
        }
    }
    public void SetItemBrush(int index)
    {
        brush = GameManager.Instance.PoolParty.GetPool("Items Pool").ObjectsToPool[index];
    }
    public void DisplayObstacleMenu(bool isDisplay)
    {
        if(isDisplay)
        {
            DoozyUI.UIElement.ShowUIElement("OBJECT_EDITOR_UI");
        }
        else
        {
            DoozyUI.UIElement.HideUIElement("OBJECT_EDITOR_UI");
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
        if (item is Obstacle)
        {
            newGameObject.transform.localScale = Size;
            Obstacle obstacle = (Obstacle)item;
            obstacle.HP = number;
        }
        else if(item is SizeItem || item is AddItem)
        {
            newGameObject.transform.localScale = new Vector2(Size.x - 1.3f, Size.y - 1.3f);
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
