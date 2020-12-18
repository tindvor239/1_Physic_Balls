using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : Item
{
    [SerializeField]
    private Color color = Color.yellow;
    [SerializeField]
    private float forceDirection = 5.0f;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Ball newBall = collision.GetComponent<Ball>();
        newBall.Sprite.color = color;
        newBall.Collider.sharedMaterial = null;
        newBall.Rigidbody.velocity = collision.transform.position;
        newBall.Trail.enabled = false;

        foreach(Ball ball in GameManager.Instance.Level.Balls)
        {
            if(collision != ball)
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
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

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
