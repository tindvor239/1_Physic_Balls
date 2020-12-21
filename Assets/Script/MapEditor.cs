using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.Level.Storage.Save(GameManager.Instance.Level);
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.Level.Storage.Load();
        }
    }
}
