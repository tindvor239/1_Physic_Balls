using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttom : MonoBehaviour
{
    [SerializeField]
    private Color color = Color.yellow;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<SpriteRenderer>().color = color;
        collision.gameObject.GetComponent<Collider2D>().sharedMaterial = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(new Vector2(rigidbody.velocity.x + Mathf.Sign(rigidbody.velocity.x) * 30f, 0));
    }
}
