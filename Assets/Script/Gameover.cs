using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover : Singleton<Gameover>
{
    [SerializeField]
    private List<GameObject> gameOverObjects = new List<GameObject>();
    public List<GameObject> GameOverObjects { get => gameOverObjects; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            gameOverObjects.Add(collision.gameObject);
            GameManager.Instance.State = GameManager.GameState.gameover;
        }
    }
}
