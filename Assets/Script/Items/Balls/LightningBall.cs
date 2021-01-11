using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LightningBall : Ball
{
    //Lightning ball within radius of obstacle that it touch and get 5 nearest obstacles -1 hp.
    [Header("Trigger's Setting")]
    [SerializeField]
    private float radius = 1.4f;
    [Header("Obstacles In Radius")]
    [SerializeField]
    private List<Obstacle> obstacles = new List<Obstacle>();
    protected CircleCollider2D circleTrigger;
    #region Properties
    public Collider2D TriggerCollider { get => circleTrigger; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        if(circleTrigger == null)
        {
            circleTrigger = gameObject.AddComponent<CircleCollider2D>();
            circleTrigger.isTrigger = true;
            circleTrigger.radius = radius;
        }
        //IGNORE EVERY COLLIDER.
        foreach(GameObject go in GameManager.Instance.Fans)
        {
            Debug.Log(go.GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(circleTrigger, go.GetComponent<Collider2D>());
        }
        foreach(Collider2D trigger in Container.Instance.Colliders)
        {
            Physics2D.IgnoreCollision(circleTrigger, trigger);
        }
        foreach(Collider2D collider in GameManager.Instance.Frames)
        {
            Physics2D.IgnoreCollision(circleTrigger, collider);
        }
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.tag == "Untagged")
        {

        }
    }
    protected override void OnHit()
    {
        foreach(Obstacle obstacle in obstacles)
        {
            obstacle.HP--;
            obstacle.OnHit();
        }
        obstacles.Clear();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            if (obstacles.Count < 5 && collision.GetComponent<Obstacle>())
            {
                obstacles.Add(collision.GetComponent<Obstacle>());
            }
        }
    }
}
