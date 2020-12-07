using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Item
{
    [SerializeField]
    private byte maxHitCount = 3;
    [SerializeField]
    private float maxhitDelay = 1.2f;
    [SerializeField]
    private float hitDelay = 0;
    private byte hitCount = 0;
    private new Rigidbody2D rigidbody;
    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        hitDelay += Time.deltaTime;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            if(hitDelay <= maxhitDelay)
            {
                hitCount++;
            }
            else
            {
                hitCount = 0;
            }
            if(hitCount >= maxHitCount)
            {
                rigidbody.AddForce(transform.position * 2, ForceMode2D.Impulse);
                hitCount = 0;
            }
        }
        hitDelay = 0;
    }
}
