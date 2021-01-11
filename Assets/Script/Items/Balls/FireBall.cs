using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Ball
{
    
    //Storage list obstacles, if in this list have any obstacle it will -1 hp.
    protected override void Awake()
    {
        base.Awake();

    }
    private void Start()
    {
        GameManager.Instance.onUpdateOneTime += Explode;
    }
    private void Update()
    {

    }
    private void Explode()
    {
        foreach(Obstacle obstacle in GameManager.Instance.HitObstacles)
        {
            obstacle.HP -= obstacle.HitCount;
            obstacle.OnHit();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.tag == "Obstacle")
        {
            Obstacle hitObstacle = collision.gameObject.GetComponent<Obstacle>();
            if(hitObstacle.HP >= 2)
            {
                hitObstacle.HitCount++;
                foreach(Obstacle obstacle in GameManager.Instance.HitObstacles)
                {
                    if(hitObstacle == obstacle)
                    {
                        return;
                    }
                }
                GameManager.Instance.HitObstacles.Add(hitObstacle);
            }
        }
    }
    private void OnDestroy()
    {
        
    }
}
