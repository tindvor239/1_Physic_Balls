using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Item
{
    [Header("Attribute Settings")]
    [SerializeField]
    protected byte maxHitCount = 5;
    [SerializeField]
    protected float maxhitDelay = 1.5f;
    [SerializeField]
    protected float hitDelay = 0;
    [SerializeField]
    protected byte hitCount = 0;
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected new Collider2D collider;
    [SerializeField]
    protected TrailRenderer trail;
    [SerializeField]
    protected SpriteRenderer sprite;
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
        bool alreadyIn = false;
        foreach (Ball ball in GameManager.Instance.Level.Balls)
        {
            if (ball == this)
            {
                alreadyIn = true;
            }
        }
        if(alreadyIn == false)
        {
            GameManager.Instance.Level.Balls.RemoveAll(ball => ball == null);
            GameManager.Instance.Level.Balls.Add(this);
        }
    }
    private void Update()
    {
        hitDelay += Time.deltaTime;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(rigidbody.gravityScale <= 1)
        {
            rigidbody.gravityScale = GameManager.Instance.Gravity;
        }
        if (collision.gameObject.tag == "Obstacle")
        {
            if (hitDelay <= maxhitDelay)
            {
                hitCount++;
            }
            else
            {
                hitCount = 0;
            }
            if (hitCount >= maxHitCount)
            {
                rigidbody.AddForce(new Vector2(transform.position.x, transform.position.y + 15f) , ForceMode2D.Impulse);
                hitCount = 0;
            }
            OnHit();
        }
        hitDelay = 0;
    }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            hitCount++;
            if(hitCount >= 6)
            {
                rigidbody.AddForce(new Vector2(transform.position.x, transform.position.y + 15f), ForceMode2D.Impulse);
                hitCount = 0;
            }
        }
    }
    protected virtual void OnHit()
    {

    }
}
