using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField]
    private float force;
    [SerializeField]
    private Transform upPos;
    private Vector2 direction;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.isTrigger == false)
        {
            Ball ball = collision.GetComponent<Ball>();
            //ball.Rigidbody.velocity = Vector2.zero;
            ball.transform.position = new Vector2(gameObject.transform.position.x, collision.transform.position.y);
            ball.Rigidbody.gravityScale = 0;
            direction = (upPos.position - collision.transform.position).normalized;
            ball.Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.isTrigger == false)
    //    {
    //        if(GameManager.Instance.Mute == false)
    //        {
    //            GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.OutSound);
    //        }
    //        foreach (Ball ball in GameManager.Instance.Level.Balls)
    //        {
    //            if (collision != ball)
    //            {
    //                if (collision.GetComponent<Ball>())
    //                    collision.GetComponent<Ball>().Rigidbody.gravityScale = 8.0f;
    //                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), false);
    //            }
    //        }
    //    }
    //    collision.GetComponent<Ball>().isAtBottom = false;
    //}
}
