using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : Item
{
    [SerializeField]
    private Color color = Color.yellow;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<SpriteRenderer>().color = color;
        collision.gameObject.GetComponent<Collider2D>().sharedMaterial = null;
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = collision.transform.position;
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
