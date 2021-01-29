using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Shooter : Singleton<Shooter>
{
    [SerializeField]
    public Ball bullet;
    [SerializeField]
    private PhysicsMaterial2D physic;
    [SerializeField]
    private float force;
    [SerializeField]
    public bool isShooting = false;
    [SerializeField]
    private List<Ball> balls;
    [SerializeField]
    private List<GameObject> containBalls = new List<GameObject>();
    [SerializeField]
    private GameObject aimCursor;
    [SerializeField]
    private float minScale = 2, maxScale = 8;
    private static float shootDelay = 0.2f;
    private static float reloadOnEndTurnDelay = 1f;
    public float reloadOnEndTurnTime = reloadOnEndTurnDelay;
    private float shootTime = shootDelay;
    [SerializeField]
    private RigidbodyType2D bodytype;
    [SerializeField]
    private float lockAngle = 80f;
    [SerializeField]
    private float gravityScale;
    [SerializeField]
    private float mass;
    [SerializeField]
    private float drag;
    [SerializeField]
    private bool isReloading = false;
    public bool isDoneShoot = true;
    [SerializeField]
    private bool isDoneSetBall = false;
    [SerializeField]
    public bool isAllIn = false;

    private Vector2 shootDirection;
    #region Properties
    public List<Ball> Balls
    {
        get => balls;
        set => balls = value;
    }
    public List<GameObject> ContainBalls
    {
        get => containBalls;
        set => containBalls = value;
    }
    public static float ReloadOnEndTurnDelay
    {
        get => reloadOnEndTurnDelay;
    }
    public GameObject AimCursor
    {
        get => aimCursor;
    }
    public float GravityScale { get => gravityScale; }
    #endregion
    private void Start()
    {
        containBalls.Clear();
    }
    // Update is called once per frame
    private void Update()
    {
        //If all balls is in start shooting.
        //If start shooting disable collision.
        //Set a closet ball = transform position to shoot.
        if(GameManager.Instance.Mode != GameManager.GameMode.editor)
        {
            if (GameManager.Instance.isEndTurn)
            {
                if(isDoneSetBall == false && balls != null)
                {
                    foreach(Ball ball in balls)
                    {
                        ball.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                    }
                    isDoneSetBall = true;
                }
                reloadOnEndTurnTime -= Time.deltaTime;
                if (reloadOnEndTurnTime <= 0)
                {
                    Reload();
                    StartCoroutine(MoveToShootPoint());
                    reloadOnEndTurnTime = reloadOnEndTurnDelay;
                }
                if (bullet != null)
                {
                   GetMouseDirection();
                }
            }
            if (isShooting)
            {
                shootTime -= Time.deltaTime;
                //To do: if balls == null.
                if (balls.Count == 0 && isReloading == false && bullet == null)
                {
                    isShooting = false;
                    shootDirection = new Vector2(0, 0);
                }
                if (shootTime <= 0)
                {
                    Shoot();
                    Reload();
                    shootTime = shootDelay;
                }
                StartCoroutine(MoveToShootPoint());
            }
            else
            {
                if(containBalls.Count != 0 && containBalls.Count == GameManager.Instance.Level.Balls.Count)
                {
                    foreach( GameObject gameObject in containBalls)
                    {
                        if(gameObject.GetComponent<Ball>())
                            balls.Add(gameObject.GetComponent<Ball>());
                    }
                    containBalls.Clear();
                }
            }
            if(balls.Count == GameManager.Instance.Level.Balls.Count && isAllIn == false)
            {
                isAllIn = true;
                GameManager.Instance.isSpawning = true;
                Bottom.Instance.Balls.Clear();
                Bottom.Instance.Directions.Clear();
            }
        }
    }
    private void GetMouseDirection()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetMouseButton(0))
            {
                aimCursor.SetActive(true);
                Vector2 mousePosition = Vector2.zero;
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float scale = minScale;
                if (mousePosition.y < 0)
                {
                    scale = Mathf.Clamp(Mathf.Abs(mousePosition.y / 3.5f), minScale, maxScale);
                }
                aimCursor.transform.localScale = new Vector2(scale, scale);
                // Aim cursor will rotate reverse direction with mouse position. So negative 1 with direction to get the same direction.
                aimCursor.transform.up = -(new Vector2(mousePosition.x * 1.8f, 5) - (Vector2)transform.position);
                // put lock rotation between from -77(291) to 77.
                if (aimCursor.transform.eulerAngles.z >= 0 && aimCursor.transform.eulerAngles.z <= 180)
                    aimCursor.transform.eulerAngles = new Vector3(aimCursor.transform.eulerAngles.x, aimCursor.transform.eulerAngles.y, Mathf.Clamp(aimCursor.transform.eulerAngles.z, 0, lockAngle));
                else
                    aimCursor.transform.eulerAngles = new Vector3(aimCursor.transform.eulerAngles.x, aimCursor.transform.eulerAngles.y, Mathf.Clamp(aimCursor.transform.eulerAngles.z, 360 - lockAngle, 360));
                shootDirection = -aimCursor.transform.up;
            }
            if (Input.GetMouseButtonUp(0))
            {
                aimCursor.SetActive(false);
                isShooting = true;
                reloadOnEndTurnTime = reloadOnEndTurnDelay;
                GameManager.Instance.isEndTurn = false;
            }
        }
    }
    public void Reload()
    {
        if (balls.Count != 0 && bullet == null && isDoneShoot == true)
        {
            bullet = balls[0];
            //if(bullet != null)
            //{
                Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
                rigidbody.bodyType = RigidbodyType2D.Static;
                bullet.GetComponent<Collider2D>().enabled = false;
                rigidbody.gravityScale = 0;
                balls.RemoveAt(0);
                isDoneShoot = false;
                isReloading = true;
            //}
        }
    }
    IEnumerator MoveToShootPoint()
    {
        if (bullet != null && isReloading)
        {
            while (bullet.transform.position != transform.position)
            {
                bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, transform.position, 0.5f);
                yield return new WaitForEndOfFrame();
            }
            if(bullet.transform.position == transform.position)
                isReloading = false;
        }
    }
    private void Shoot()
    {
        if (bullet != null && isReloading == false)
        {
            bullet.Rigidbody.mass = mass;
            bullet.Rigidbody.angularDrag = drag;
            bullet.Rigidbody.bodyType = bodytype;
            bullet.Rigidbody.gravityScale = gravityScale;
            bullet.Rigidbody.AddForce(shootDirection * force, ForceMode2D.Impulse);
            bullet.Collider.enabled = true;
            bullet.Collider.sharedMaterial = physic;
            bullet.Trail.enabled = true;
            if(bullet is LightningBall)
            {
                LightningBall lightningBullet = (LightningBall)bullet;
                lightningBullet.TriggerCollider.enabled = true;
            }
            foreach(Ball gameobject in GameManager.Instance.Level.Balls)
            {
                if (bullet != gameobject)
                {
                    Ball bulletBall = bullet.GetComponent<Ball>();
                    Physics2D.IgnoreCollision(bulletBall.Collider, gameobject.Collider, true);
                }
            }
            isAllIn = false;
            isDoneSetBall = false;
            bullet = null;
            isDoneShoot = true;
        }
    }
}
