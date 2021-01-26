using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StorageJson
{
    #region Member Field
    [SerializeField]
    private string folderName;
    [SerializeField]
    private string fileName;
    [SerializeField]
    private string fileExtension;
    [SerializeField]
    private LevelPackage convertedLevel;
    #endregion
    #region Properties
    public string FileName
    {
        get
        {
            if(convertedLevel != null)
            {
                fileName = $"level_{convertedLevel.name}";
            }
            return fileName;
        }
    }
    public string FolderName { get => folderName; }
    public LevelPackage ConvertedLevel { get => convertedLevel; set => convertedLevel = value; }
    protected string Path
    {
        get
        {
            string result = "";
            if (fileExtension.IndexOf('.') != -1)
            {
                result = fileExtension.Substring(fileExtension.IndexOf('.') + 1, fileExtension.Length - (fileExtension.IndexOf('.') + 1));
            }
            return string.Format("{0}/{1}.{2}", Application.dataPath + "/Resources/" + folderName, fileName, result);
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
            Directory.CreateDirectory(Path);
        }
        WriteFile(level);
    }
    public void ConvertJsonToObject(string path)
    {
        if(File.Exists(path))
        {
            List<string> fieldList = new List<string>();
            List<string> toRemove = new List<string>();
            #region Convert To ScriptableObject
            string level = "";
            level = GetLevelInfo(path, fieldList);
            //recorrect string into items list
            for (int index = 0; index < fieldList.Count; index++)
            {
                if(index + 1 < fieldList.Count)
                {
                    //if obstacle will get 6 fields.
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
                    //if dead item will get 6 fields
                    else if(fieldList[index].IndexOf("\"type\":\"DeadItem\"") != -1)
                    {
                        for (int fieldIndex = index + 1; fieldIndex < index + 6; fieldIndex++)
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
                    //if additem or sizeitem will get 6 fields.
                    else if(fieldList[index].IndexOf("\"type\":\"AddItem\"") != -1 || fieldList[index].IndexOf("\"type\":\"SizeItem\"") != -1)
                    {
                        for (int fieldIndex = index + 1; fieldIndex < index + 6; fieldIndex++)
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
            //remove the unused string.
            foreach(string removeString in toRemove)
            {
                fieldList.Remove(removeString);
            }
            BaseLevel baseLevel = (BaseLevel)JsonUtility.FromJson(level, typeof(BaseLevel));
            convertedLevel.Pack(baseLevel, fieldList);
            #endregion
        }
    }
    private string GetLevelInfo(string path, List<string> levelInfos)
    {
        string data = File.ReadAllText(path);
        string[] jsonFields = data.Split('}');
        string level = "";
        //split json string to array.
        for (int index = 0; index < jsonFields.Length; index++)
        {
            jsonFields[index] += '}';
        }
        //set array to list, get level info
        for (int index = 1; index < jsonFields.Length; index++)
        {
            if (jsonFields[index] != "}")
                levelInfos.Add(jsonFields[index]);
            level = jsonFields[0];
        }
        return level;
    }
    private void WriteFile(Level level)
    {
        string json = "";
        if (level.Balls != null && level.Items != null)
        {
            GameManager.Instance.Level.Row = Spawner.Instance.Obstacles.rows.Count;
            GameManager.Instance.Level.Column = Spawner.Instance.Obstacles.rows[0].columns.Count;
            //pack level into json;
            BaseLevel baseLevel = new BaseLevel();
            baseLevel.Pack(GameManager.Instance.Level);
            json += JsonUtility.ToJson(baseLevel);
            foreach (Items items in Spawner.Instance.Obstacles.rows)
            {
                foreach (Item item in items.columns)
                {
                    if (item != null)
                    {
                        GameManager.Instance.Level.Items.Add(item);
                    }
                }
            }
            foreach (Item item in GameManager.Instance.Level.Items)
            {
                Package package = new Package();
                if(item is Obstacle)
                {
                    package = new CreaturePackage();
                    Obstacle obstacle = (Obstacle)item;
                    package.Pack(obstacle.gameObject);
                    package.type = "Obstacle";
                }
                if(item is DeadItem)
                {
                    package = new DeadPackage();
                    DeadItem deadItem = (DeadItem)item;
                    package.Pack(deadItem.gameObject);
                    package.type = "DeadItem";
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
                //Debug.Log(JsonUtility.ToJson(package));
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
public class Package
{
    public string name;
    public string type;
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 offset;
    public Vector2 size;
    public int row = 0, column = 0;
    public virtual void Pack(GameObject gameObject)
    {
        name = gameObject.name;
        position = gameObject.transform.position;
        rotation = gameObject.transform.rotation;
        size = gameObject.transform.localScale;
        if(gameObject.GetComponent<Collider2D>())
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            offset = collider.offset;
        }
        for(int row = 0; row < Spawner.Instance.Obstacles.rows.Count; row++)
        {
            for(int column = 0; column < Spawner.Instance.Obstacles.rows[row].columns.Count; column++)
            {
                if(Spawner.Instance.Obstacles.rows[row].columns[column].gameObject == gameObject)
                {
                    this.row = row;
                    this.column = column;
                }
            }
        }
    }
    public virtual void Unpack(GameObject gameObject)
    {
        gameObject.name = name;
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.localScale = size;
        if(gameObject.GetComponent<Collider2D>())
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            collider.offset = offset;
        }
    }
}
public class BaseLevel
{
    public string name;
    public byte stars;
    public int row, column;
    public byte star1;
    public byte star2;
    public byte star3;
    public byte turnCount;
    public bool canMoveUp;
    public int balls;
    public void Pack(Level level)
    {
        name = level.Name;
        stars = level.Stars;
        row = level.Row;
        column = level.Column;
        star1 = level.Points[0];
        star2 = level.Points[1];
        star3 = level.Points[2];
        turnCount = level.TurnCount;
        canMoveUp = level.CanMoveUp;
        balls = level.Balls.Count;
    }
}
public class CreaturePackage : Package
{
    public int hp;
    public string geometry;
    public override void Pack(GameObject gameObject)
    {
        base.Pack(gameObject);
        if(gameObject.GetComponent<Obstacle>())
        {
            Obstacle obstacle = gameObject.GetComponent<Obstacle>();
            hp = obstacle.HP;
            geometry = obstacle.Geometry.ToString();
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
public class DeadPackage : Package
{
    public string geometry;
    public override void Pack(GameObject gameObject)
    {
        base.Pack(gameObject);
        if (gameObject.GetComponent<DeadItem>())
        {
            DeadItem item = gameObject.GetComponent<DeadItem>();
            geometry = item.Geometry.ToString();
        }
    }

    public override void Unpack(GameObject gameObject)
    {
        base.Unpack(gameObject);
    }
}