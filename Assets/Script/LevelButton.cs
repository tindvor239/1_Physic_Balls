using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private new Text name;
    [SerializeField]
    private byte star;
    
    public void OnSelected()
    {
        string path = string.Format("{0}/{1}/{2}.json", Application.persistentDataPath, GameManager.Instance.LevelFolder, name);
        if (File.Exists(path))
        {
            //Load file saves.
        }
    }
}
