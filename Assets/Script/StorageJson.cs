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
            return string.Format("{0}/{1}.{2}", Application.dataPath + "/" + folderName, fileName, result);
        }
    }
    #endregion
    #region Method
    public void Save(Level level)
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
    public virtual void Load()
    {
        if(File.Exists(Path))
        {
            //Will be fixed
            PoolParty poolParty = GameManager.Instance.PoolParty;
            string data = File.ReadAllText(Path);
            string[] fieldData = data.Split('}');
            List<string> fieldList = new List<string>();
            List<string> toRemove = new List<string>();
            string level = "";
            //split json string to array.
            for(int index = 0; index < fieldData.Length; index++)
            {
                fieldData[index] += '}';
            }
            //set array to list, get level info
            for(int index = 1; index < fieldData.Length; index++)
            {
                if(fieldData[index] != "}")
                    fieldList.Add(fieldData[index]);
                level = fieldData[0];
            }
            //recorrect string into items list
            for (int index = 0; index < fieldList.Count; index++)
            {
                if(index + 1 < fieldList.Count)
                {
                    //if obstacle will get 5 fields becuz dont have HP.
                    if(fieldList[index].IndexOf("\"type\":\"Obstacle\"") != -1)
                    {
                        for(int fieldIndex = index + 1; fieldIndex < index + 6; fieldIndex++)
                        {
                            if(fieldIndex < fieldList.Count)
                            {
                                if (fieldList[index].LastIndexOf('}') == fieldList[index].Length - 1 && fieldList[fieldIndex].IndexOf(',') == 0)
                                {
                                    fieldList[index] += fieldList[fieldIndex];
                                    toRemove.Add(fieldList[fieldIndex]);
                                }
                            }
                        }
                    }
                    //if additem or sizeitem will get 4 fields.
                    else if(fieldList[index].IndexOf("\"type\":\"AddItem\"") != -1 || fieldList[index].IndexOf("\"type\":\"SizeItem\"") != -1)
                    {
                        for (int fieldIndex = index + 1; fieldIndex < index + 5; fieldIndex++)
                        {
                            if (fieldIndex < fieldList.Count)
                            {
                                if (fieldList[index].LastIndexOf('}') == fieldList[index].Length - 1 && fieldList[fieldIndex].IndexOf(',') == 0)
                                {
                                    fieldList[index] += fieldList[fieldIndex];
                                    toRemove.Add(fieldList[fieldIndex]);
                                }
                            }
                        }
                    }
                }
            }
            foreach(string st in toRemove)
            {
                fieldList.Remove(st);
            }
            LevelPackage levelPackage = (LevelPackage)JsonUtility.FromJson(level, typeof(LevelPackage));
            levelPackage.Unpack(GameManager.Instance.Level);
            for(int count = 0; count < levelPackage.balls; count++)
            {
                //To do: spawn ball.
                Ball ball = GameManager.Instance.CreateObject(GameManager.Instance.GameScene.transform, GameManager.Instance.Level.BallPrefab).GetComponent<Ball>();
                if (count == 0)
                {
                    ball.transform.position = Shooter.Instance.transform.position;
                }
                else
                {
                    ball.transform.position = GameManager.Instance.SpawnBall.transform.position;
                }
                GameManager.Instance.Level.Balls.Add(ball);
                Shooter.Instance.Balls.Add(ball);
            }
            Shooter.Instance.Reload();
            foreach(string s in fieldList)
            {
                string type = "";
                if(s.IndexOf("\"type\":\"Obstacle\"") != -1)
                {
                    type = "Obstacles Pool";
                }
                else if(s.IndexOf("\"type\":\"AddItem\"") != -1 || s.IndexOf("\"type\":\"SizeItem\"") != -1)
                {
                    type = "Items Pool";
                }
                GameObject item = poolParty.CreateItem(poolParty.GetPool(type), GameManager.Instance.transform.position, 1, GameManager.Instance.transform);
            }
            //CreaturePackage package = (CreaturePackage)JsonUtility.FromJson(data, typeof(CreaturePackage));
            //package.Unpack(something);
            //Debug.Log(string.Format("name: {0}, position: {1}, rotation: {2}, hp: {3}", package.name, package.position, package.rotation, package.hp));
        }
    }
    public void WriteFile(Level level)
    {
        string json = "";
        if (level.Balls != null && level.Items != null)
        {
            //To do: write name.
            string name = GameManager.Instance.Level.Name;
            //To do: write star.
            byte star = GameManager.Instance.Level.Star;
            //To do: write ball count.
            byte ballCount = (byte)GameManager.Instance.Level.Balls.Count;
            json += "{" + string.Format("{0},{1},{2}", StorageField("name", name), StorageField("stars", star), StorageField("balls", ballCount)) + "}";
            foreach(Item item in GameManager.Instance.Level.Items)
            {
                Package package = new Package();
                if(item is Obstacle)
                {
                    package = new CreaturePackage();
                    Obstacle obstacle = (Obstacle)item;
                    package.Pack(obstacle.gameObject);
                    package.type = "Obstacle";
                }
                if(item is AddItem)
                {
                    AddItem addItem = (AddItem)item;
                    package.Pack(addItem.gameObject);
                    package.type = "AddItem";
                }
                if(item is SizeItem)
                {
                    SizeItem sizeItem = (SizeItem)item;
                    package.Pack(sizeItem.gameObject);
                    package.type = "SizeItem";
                }
                Debug.Log(JsonUtility.ToJson(package));
                json += JsonUtility.ToJson(package);
            }
            File.WriteAllText(Path, json);
        }
    }
    public string StorageField(string fieldName, object value)
    {
        return string.Format("\"{0}\":\"{1}\"", fieldName, value);
    }
    #endregion
}
public class LevelPackage
{
    public string name;
    public byte stars;
    public byte balls;
    public virtual void Unpack(Level level)
    {
        level.Name = name;
        level.Star = stars;
    }
}
public class Package
{
    public string name;
    public string type;
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
    public string geometry;
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