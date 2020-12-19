using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[Serializable]
public class StorageJson
{
    #region Member Field
    [SerializeField]
    protected string folderName;
    [SerializeField]
    protected string fileName;
    [SerializeField]
    protected string fileExtension;

    [SerializeField]
    protected GameObject ballPrefab;
    [SerializeField]
    protected List<GameObject> obstaclesPrefab;
    #endregion
    #region Properties
    public string FileName { get => fileName; }
    protected string Path
    {
        get
        {
            string result = "";
            if (fileExtension.IndexOf('.') != -1)
            {
                result = fileExtension.Substring(fileExtension.IndexOf('.') + 1, fileExtension.Length - (fileExtension.IndexOf('.') + 1));
            }
            return string.Format("{0}/{1}.{2}", Application.persistentDataPath, fileName, result);
        }
    }
    #endregion
    #region Method
    protected virtual void Save(Level level)
    {
        if (File.Exists(Path))
        {
            Debug.Log("file already exist!");
        }
        else
        {
            Debug.Log("file not exist!!");
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        WriteFile(level);
    }
    protected virtual void Load()
    {
        if(File.Exists(Path))
        {
            //Will be fixed
            PoolParty poolParty = GameManager.Instance.PoolParty;
            GameObject something = poolParty.CreateItem(poolParty.GetPool("Obstacles Pool"), GameManager.Instance.transform.position, 1, GameManager.Instance.transform);
            string data = File.ReadAllText(Path);
            CreaturePackage package = (CreaturePackage)JsonUtility.FromJson(data, typeof(CreaturePackage));
            package.Unpack(something);
            Debug.Log(string.Format("name: {0}, position: {1}, rotation: {2}, hp: {3}", package.name, package.position, package.rotation, package.hp));
        }
    }
    public void WriteFile(Level level)
    {
        string json = "";
        Debug.Log(Path);
        if (level.Balls != null && level.Obstacles != null)
        {
            //To do: write name.
            string name = GameManager.Instance.Level.Name;
            json += JsonUtility.ToJson(name);
            //To do: write star.
            byte star = GameManager.Instance.Level.Star;
            json += JsonUtility.ToJson(star);
            //To do: write ball count.
            byte ballCount = (byte)GameManager.Instance.Level.Balls.Count;
            json += JsonUtility.ToJson(ballCount);
            foreach(Obstacle obstacle in GameManager.Instance.Level.Obstacles)
            {
                CreaturePackage package = new CreaturePackage();
                package.Pack(obstacle.gameObject);
                Debug.Log(JsonUtility.ToJson(package));
                json += JsonUtility.ToJson(package);
            }
            File.WriteAllText(Path, json);
        }
    }
    #endregion
}

public class Package
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 offset;
    public Vector2 size;
    public virtual void Pack(GameObject gameObject)
    {
        name = gameObject.name;
        position = gameObject.transform.position;
        rotation = gameObject.transform.rotation;
        if(gameObject.GetComponent<Collider2D>())
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            offset = collider.offset;
            if(collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
                size = boxCollider.size;
            }
        }
    }
    public virtual void Unpack(GameObject gameObject)
    {
        gameObject.name = name;
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        if(gameObject.GetComponent<Collider2D>())
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            collider.offset = offset;
            if(collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
                size = boxCollider.size;
            }
        }
    }
}

public class CreaturePackage : Package
{
    public uint hp;
    public override void Pack(GameObject gameObject)
    {
        base.Pack(gameObject);
        if(gameObject.GetComponent<Obstacle>())
        {
            Obstacle obstacle = gameObject.GetComponent<Obstacle>();
            Debug.Log(obstacle.HP);
            hp = obstacle.HP;
        }
    }
    public override void Unpack(GameObject gameObject)
    {
        base.Unpack(gameObject);
        if(gameObject.GetComponent<Obstacle>())
        {
            Obstacle obstacle = gameObject.GetComponent<Obstacle>();
            obstacle.HP = hp;
        }
    }
}