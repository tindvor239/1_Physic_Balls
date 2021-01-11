using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBall : Ball
{
    //It will need a pool to storage laser.
    //Everytime it hit, it will spawn a laser object (laser object will -1 hp of any obstacles)
    //After 1s laser will disappear.
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.tag == "Obstacle")
        {
            Pool pool = GameManager.Instance.PoolParty.GetPool("Lasers Pool");
            GameObject laser = null;
            if (pool.CanExtend)
            {
                laser = GameManager.Instance.PoolParty.CreateItem(pool, collision.transform.position, 0, GameManager.Instance.GameScene.transform);
            }
            else
            {
                foreach(GameObject gameObject in pool.ObjectsPool)
                {
                    if(gameObject.activeInHierarchy == false)
                    {
                        laser = pool.GetOutOfPool(gameObject, collision.transform.position);
                    }
                }
            }
        }
    }
}
