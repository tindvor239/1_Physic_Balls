using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapEditor : Singleton<MapEditor>
{
    [SerializeField]
    private GameObject gridPrefab, gridIndexerPrefab;
    [SerializeField]
    private List<GameObject> gridRowIndexeres = new List<GameObject>();
    [SerializeField]
    private Spawner spawner;
    [SerializeField]
    private uint row, column;
    private uint lastRow, lastColumn;
    [SerializeField]
    private Vector2 space, size;
    #region Properties
    public GameObject GridPrefab { get => gridPrefab; }
    public GameObject GridIndexerPrefab { get => gridIndexerPrefab; }
    public List<GameObject> GridRowIndexeres { get => gridRowIndexeres; }
    public Spawner Spawner { get => spawner; }
    public uint Row { get => row; }
    public uint Column { get => column; }
    public Vector2 Space { get => space; }
    #endregion
    // Update is called once per frame
    private void Start()
    {
        spawner = Spawner.Instance;
        row = (uint)Spawner.Instance.Obstacles.rows.Count;
        column = (uint)Spawner.Instance.Obstacles.rows[0].columns.Count;
        SetRowPrefab();
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
        if(row != lastRow)
        {
            lastRow = row;
            Items[] items = new Items[row];
            Spawner.Instance.Obstacles.rows = items.ToList();
        }
    }
    private void SetRowPrefab()
    {
        DestroyExistInList();
        for (int index = 0; index <= row; index++)
        {
            gridRowIndexeres.Add(SpawnItem(GameManager.Instance.PoolParty.GetPool("Tile Indexeres Pool"), Spawner.Instance.transform.position, 0));
        }
    }
    private void DestroyExistInList()
    {
        foreach (GameObject gameObject in gridRowIndexeres)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        gridRowIndexeres.RemoveAll(gameObject => gameObject == null);
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
                if (gameObject.activeInHierarchy == false &&
                    gameObject.GetComponent<Item>().GetType() == pool.ObjectsToPool[index].GetComponent<Item>().GetType() && isGotIt == false)
                {
                    newGameObject = pool.GetOutOfPool(gameObject, position);
                    isGotIt = true;
                }
            }
        }
        return newGameObject;
    }
}
