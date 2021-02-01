using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHandle : Singleton<BackHandle>
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.State == GameManager.GameState.play)
            {
                GameManager.Instance.Pause();
            }
            else if(GameManager.Instance.State == GameManager.GameState.pause)
            {
                GameManager.Instance.Continue();
            }
            else if(GameManager.Instance.State == GameManager.GameState.start)
            {
                //Do you really want to quit.
            }
        }
    }
}
