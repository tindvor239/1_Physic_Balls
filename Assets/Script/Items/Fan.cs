using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Bottom
{
    [SerializeField]
    private float force;
    [SerializeField]
    private float pushForce;
    [SerializeField]
    private Vector2 direction;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        ball.Rigidbody.velocity = Vector2.zero;
        ball.Rigidbody.transform.position = new Vector2(transform.position.x, collision.transform.position.y);
        ball.Rigidbody.gravityScale = 0;
        ball.Rigidbody.AddForce(new Vector2(collision.transform.position.x, 2 * force), ForceMode2D.Impulse);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (Ball ball in GameManager.Instance.Level.Balls)
        {
            if (collision != ball)
            {
                if(collision.GetComponent<Ball>())
                    collision.GetComponent<Ball>().Rigidbody.gravityScale = 8.0f;
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), false);
            }
        }
    }
}
