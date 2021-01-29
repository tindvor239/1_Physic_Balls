﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover : Singleton<Gameover>
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            GameManager.Instance.State = GameManager.GameState.gameover;
        }
    }
}
