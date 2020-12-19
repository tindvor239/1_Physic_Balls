using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    [SerializeField]
    private Level level;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            //To do Save.
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            //To do Load.
        }
    }
}
