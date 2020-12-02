﻿using System;
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
    private static float shootTime = 0.25f;
    private static float reloadOnEndTurnDelay = 2f;
    private float currentTime = shootTime;

    private bool isReloading = false;
    private bool isDoneShoot = false;
    [SerializeField]
    private bool isAllIn = false;

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
            reloadOnEndTurnDelay -= Time.fixedDeltaTime;
            if(reloadOnEndTurnDelay <= 0)
            {
                Reload();
                StartCoroutine(MoveToShootPoint());
                reloadOnEndTurnDelay = 2;
            }
            if (bullet != null)
            {
               GetMouseDirection();
            }
        }
        if (isShooting)
        {
            //To do: if balls == null.
            if (balls.Count == 0 && isReloading == false && bullet == null)
            {
                isShooting = false;
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
        if(Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            aimCursor.SetActive(true);
            Vector2 mousePosition = Vector2.zero;
            if (Input.GetMouseButton(0))
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                mousePosition = touch.position;
            }
            float scale = Mathf.Clamp(Vector2.Distance(mousePosition, transform.position), minScale, maxScale);
            aimCursor.transform.localScale = new Vector2(scale, scale);
            // Aim cursor will rotate reverse direction with mouse position. So negative 1 with direction to get the same direction.
            aimCursor.transform.up = -(mousePosition - (Vector2)transform.position);
            // put lock rotation between from -77(291) to 77.
            if(aimCursor.transform.eulerAngles.z >= 0 && aimCursor.transform.eulerAngles.z <= 180)
                aimCursor.transform.eulerAngles = new Vector3(aimCursor.transform.eulerAngles.x, aimCursor.transform.eulerAngles.y, Mathf.Clamp(aimCursor.transform.eulerAngles.z, 0, 77));
            else
                aimCursor.transform.eulerAngles = new Vector3(aimCursor.transform.eulerAngles.x, aimCursor.transform.eulerAngles.y, Mathf.Clamp(aimCursor.transform.eulerAngles.z, 291, 360));
            shootDirection = -aimCursor.transform.up;
        }
        if(Input.GetMouseButtonUp(0))
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
        }
    }
    IEnumerator MoveToShootPoint()
    {
        if (bullet != null && isReloading)
        {
            while (bullet.transform.position != transform.position)
            {
                bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, transform.position, 0.2f);
                yield return new WaitForEndOfFrame();
            }
            isReloading = false;
        }
    }
    private void Shoot()
    {
        if (bullet != null && isReloading == false)
        {
            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.mass = 1;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            bullet.GetComponent<Collider2D>().enabled = true;
            rigidbody.gravityScale = 2;
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
        isDoneShoot = true;
    }
}
