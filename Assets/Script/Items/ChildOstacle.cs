using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildOstacle : MonoBehaviour
{
    private Obstacle parent;
    private void Start()
    {
        parent = transform.parent.GetComponent<Obstacle>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        parent.OnChildCollisionEnter(collision);
    }
}
