using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.isTrigger == false)
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
