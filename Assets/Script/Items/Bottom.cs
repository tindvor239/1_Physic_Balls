﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : Singleton<Bottom>
{
    [SerializeField]
    private Color color = Color.yellow;
    [SerializeField]
    private float forceDirection = 5.0f;
    [SerializeField]
    private Collider2D trigger;
    #region Properties
    public Collider2D Trigger { get => trigger; }
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false)
        {
            Ball newBall = collision.GetComponent<Ball>();
            newBall.Sprite.color = color;
            newBall.Collider.sharedMaterial = null;
            newBall.Rigidbody.velocity = collision.transform.position;
            newBall.Trail.enabled = false;

            foreach(Ball ball in GameManager.Instance.Level.Balls)
            {
                if(newBall != ball)
                    Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
            }
            if(newBall is LightningBall)
            {
                LightningBall lightningBall = (LightningBall)newBall;
                lightningBall.TriggerCollider.enabled = false;
            }

            Vector2 pushDirection = new Vector2();
            if (collision.transform.position.x >= gameObject.transform.position.x)
            {
                pushDirection = new Vector2(-(forceDirection * 5), collision.transform.position.y);
            }
            else
            {
                pushDirection = new Vector2(forceDirection * 5, collision.transform.position.y);
            }    
            
            newBall.Rigidbody.AddForce(pushDirection, ForceMode2D.Impulse);
        }
    }
}
