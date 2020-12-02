using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Shooter : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private PhysicsMaterial2D physic;
    [SerializeField]
    private float force;
    [SerializeField]
    private bool isShooting = false;
    [SerializeField]
    private List<GameObject> balls;
    [SerializeField]
    private List<GameObject> containBalls;
    [SerializeField]
    private GameObject aimCursor;
    [SerializeField]
    private float minScale = 2, maxScale = 8;
    private float maxY = 5;
    private static float shootTime = 0.25f;
    private float currentTime = shootTime;

    private bool isReloading = false;
    private bool isDoneShoot = false;
    private bool isAllIn = false;
    [SerializeField]
    private int count = 0;

    private Vector2 shootDirection;
    #region Singleton
    public static Shooter Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    #region Properties
    public List<GameObject> Balls
    {
        get => balls;
        set => balls = value;
    }
    public List<GameObject> ContainBalls
    {
        get => containBalls;
        set => containBalls = value;
    }
    #endregion
    private void Start()
    {
        containBalls.Clear();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        currentTime -= Time.fixedDeltaTime;
        //If all balls is in start shooting.
        //If start shooting disable collision.
        //Set a closet ball = transform position to shoot.
        if (GameManager.Instance.isEndTurn)
        {
            Reload();
            StartCoroutine(MoveToShootPoint());
            //Debug.Log("Is End Turn");
            if (bullet != null)
            {
               GetMouseDirection();
            }
        }
        if (isShooting)
        {   
            if (count == GameManager.Instance.Balls.Count && isReloading == false)
            {
                isShooting = false;
                count = 0;
                shootDirection = new Vector2(0, 0);
            }
            if (currentTime <= 0)
            {
                Shoot();
                Reload();
                currentTime = shootTime;
            }
            StartCoroutine(MoveToShootPoint());
        }
        else
        {
            if(containBalls.Count == GameManager.Instance.Balls.Count && containBalls.Count != 0)
            {
                foreach( GameObject gameObject in containBalls)
                {
                    balls.Add(gameObject);
                }
                containBalls.Clear();
                Debug.Log("Shooting");
            }
        }
        if(balls.Count == GameManager.Instance.Balls.Count && isAllIn == false)
        {
            isAllIn = true;
            GameManager.Instance.isSpawning = true;
        }
    }

    private void GetMouseDirection()
    {
        Vector2 mousePosition = new Vector2();
        if(Input.GetMouseButton(0))
        {
            aimCursor.SetActive(true);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = new Vector2(mousePosition.x, Mathf.Clamp(mousePosition.y, -40, maxY));

            shootDirection = mousePosition - (Vector2)transform.position;
            if(shootDirection.y >= 5)
            {
                shootDirection = new Vector2(shootDirection.x, -shootDirection.y) * 3;
            }
            float scale = Mathf.Clamp(-mousePosition.y, minScale, maxScale);
            aimCursor.transform.localScale = new Vector2(scale, scale);
            aimCursor.transform.up = -shootDirection;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            aimCursor.SetActive(false);
            isShooting = true;
        }
    }
    private void Reload()
    {
        if (balls.Count != 0 && bullet == null && isDoneShoot == true)
        {
            bullet = balls[0].gameObject;
            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Static;
            bullet.GetComponent<Collider2D>().enabled = false;
            rigidbody.gravityScale = 0;
            balls.RemoveAt(0);
            isDoneShoot = false;
            isReloading = true;
            Debug.Log("Reloading ");
        }
    }
    IEnumerator MoveToShootPoint()
    {
        if (bullet != null && isReloading)
        {
            //Debug.Log("Begin");
            //if (bullet.transform.position != transform.position)
            //{
            //bullet.transform.position = Vector2.MoveTowards(bullet.transform.position,transform.position, 0.5f);
            //Debug.Log("Moving " + bullet.name);
            //}
            //if(bullet.transform.position == transform.position)
            //{
            //isReloading = false;
            //Debug.Log("Done Reload " + bullet.name);
            //}
            while (bullet.transform.position != transform.position)
            {
                bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, transform.position, 0.5f);
                //Debug.Log("In While");
                yield return new WaitForEndOfFrame();
            }
            isReloading = false;
            //Debug.Log("Return");
        }
    }
    private void Shoot()
    {
        if (bullet != null && isReloading == false)
        {
            count++;
            //Debug.Log(count);
            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            bullet.GetComponent<Collider2D>().enabled = true;
            rigidbody.gravityScale = 1;
            Debug.Log(shootDirection);
            bullet.GetComponent<Rigidbody2D>().AddForce(shootDirection * force, ForceMode2D.Impulse);
            bullet.GetComponent<Collider2D>().sharedMaterial = physic;
            foreach(GameObject ball in GameManager.Instance.Balls)
            {
                if (bullet != ball)
                    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
            }
        }
        isAllIn = false;
        GameManager.Instance.isEndTurn = false;
        bullet = null;
        //Debug.Log("Shoot");
        isDoneShoot = true;
    }
}
