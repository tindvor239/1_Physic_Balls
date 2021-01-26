using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadItem : Obstacle
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Gameover")
        {
            GameManager.Instance.PoolParty.GetPool("Dead Obstacles Pool").GetBackToPool(gameObject, GameManager.Instance.transform.position);
        }
    }
}
