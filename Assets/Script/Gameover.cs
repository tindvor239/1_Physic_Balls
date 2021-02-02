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
            if(collision.GetComponent<Obstacle>() is DeadItem)
            {
                DeadItem deadItem = collision.GetComponent<DeadItem>();
                Destroy(deadItem.gameObject);
            }
            else
            {
                gameOverObjects.Add(collision.gameObject);
                GameManager.Instance.State = GameManager.GameState.gameover;
            }
            SetObstacleOutArray(collision.gameObject);
        }
        if(collision.GetComponent<AddItem>() || collision.GetComponent<SizeItem>())
        {
            GameManager.Instance.PoolParty.GetPool("Items Pool").GetBackToPool(collision.gameObject, GameManager.Instance.transform.position);
            SetObstacleOutArray(collision.gameObject);
        }
    }

    private void SetObstacleOutArray(GameObject gameObject)
    {
        for (int i = Spawner.Instance.Obstacles.rows.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < Spawner.Instance.Obstacles.rows[i].columns.Count; j++)
            {
                if (Spawner.Instance.Obstacles.rows[i].columns[j] != null && gameObject.GetInstanceID() == Spawner.Instance.Obstacles.rows[i].columns[j].gameObject.GetInstanceID())
                {
                    Spawner.Instance.Obstacles.rows[i].columns[j] = null;
                }
            }
        }
    }
}
