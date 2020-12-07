using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : Item
{
    [SerializeField]
    private Color color = Color.yellow;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<SpriteRenderer>().color = color;
        collision.GetComponent<Collider2D>().sharedMaterial = null;
        collision.GetComponent<Rigidbody2D>().velocity = collision.transform.position;
        foreach(GameObject ball in GameManager.Instance.Balls)
        {
            if(collision != ball)
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
        }
        Rigidbody2D rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2();
        if (collision.transform.position.x >= gameObject.transform.position.x)
        {
            force = new Vector2(-5, collision.transform.position.y);
        }
        else
            force = new Vector2(5, collision.transform.position.y);
        rigidbody.AddForce(force * 10f, ForceMode2D.Impulse);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
