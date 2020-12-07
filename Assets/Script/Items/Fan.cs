﻿using System.Collections;
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
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        collision.gameObject.transform.position = new Vector2(collision.gameObject.transform.position.x + Time.deltaTime, collision.gameObject.transform.position.y + Time.deltaTime * force);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        Rigidbody2D rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 5;
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * pushForce, ForceMode2D.Impulse);
        foreach (GameObject ball in GameManager.Instance.Balls)
        {
            if (collision != ball)
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), false);
        }
    }
}
