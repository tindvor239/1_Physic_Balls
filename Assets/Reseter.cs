using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reseter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Ball>())
        {
            if (GameManager.Instance.Mute == false)
            {
                GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.OutSound);
            }
            foreach (Ball ball in GameManager.Instance.Level.Balls)
            {
                if (collision != ball)
                {
                    if (collision.GetComponent<Ball>())
                        collision.GetComponent<Ball>().Rigidbody.gravityScale = 8.0f;
                    Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), false);
                }
            }
            collision.GetComponent<Ball>().isAtBottom = false;
        }
    }
}
