using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Item
{
    [SerializeField]
    private byte maxHitCount = 10;
    [SerializeField]
    private float maxhitDelay = 1.5f;
    [SerializeField]
    private float hitDelay = 0;
    [SerializeField]
    private byte hitCount = 0;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private TrailRenderer trail;
    private SpriteRenderer sprite;
    #region Properties
    public TrailRenderer Trail { get => trail; }
    public Rigidbody2D Rigidbody { get => rigidbody; }
    public SpriteRenderer Sprite { get => sprite; }
    public Collider2D Collider { get => collider; }
    #endregion
    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
    private void Update()
    {
        //hitDelay += Time.deltaTime;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(rigidbody.gravityScale <= 1)
        {
            rigidbody.gravityScale = GameManager.Instance.Gravity;
        }
        //if(collision.gameObject.tag == "Obstacle")
        //{
        //    if(hitDelay <= maxhitDelay)
        //    {
        //        hitCount++;
        //    }
        //    else
        //    {
        //        hitCount = 0;
        //    }
        //    if(hitCount >= maxHitCount)
        //    {
        //        rigidbody.AddForce(collision.transform.position * 1.2f, ForceMode2D.Impulse);
        //        hitCount = 0;
        //    }
        //}
        //hitDelay = 0;
    }
}
