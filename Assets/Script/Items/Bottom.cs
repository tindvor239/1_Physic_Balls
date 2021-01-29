using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : Singleton<Bottom>
{
    [SerializeField]
    private Color color = Color.yellow;
    [SerializeField]
    private float force = 5.0f;
    [SerializeField]
    private Collider2D trigger;
    [SerializeField]
    private Transform leftPos;
    [SerializeField]
    private Transform rightPos;
    [SerializeField]
    private List<Ball> balls;
    [SerializeField]
    private List<Vector2> directions;
    #region Properties
    public Collider2D Trigger { get => trigger; }
    public List<Ball> Balls { get => balls; }
    public List<Vector2> Directions { get => directions; }
    #endregion
    private void Update()
    {
        //if(balls != null)
        //{
        //    count += Time.deltaTime;
        //    if(count >= 4f)
        //    {
        //        foreach(Ball ball in balls)
        //        {
        //            ball.AtBottom();
        //        }
        //        count = 0;
        //    }
        //}
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.isTrigger == false)
        //{

        //    foreach (Ball ball in GameManager.Instance.Level.Balls)
        //    {
        //        if (newBall != ball)
        //            Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
        //    }
        //    //if(newBall is LightningBall)
        //    //{
        //    //    LightningBall lightningBall = (LightningBall)newBall;
        //    //    lightningBall.TriggerCollider.enabled = false;
        //    //}
        //    Vector2 pushDirection = new Vector2();
        //    if (collision.transform.position.x >= gameObject.transform.position.x)
        //    {
        //        pushDirection = new Vector2(-(forceDirection * 5), collision.transform.position.y);
        //    }
        //    else
        //    {
        //        pushDirection = new Vector2(forceDirection * 5, collision.transform.position.y);
        //    }
        //    newBall.Rigidbody.AddForce(pushDirection, ForceMode2D.Impulse);
        //    newBall.AtBottom();
        //    balls.Add(newBall);
        //}
        balls.Add(collision.GetComponent<Ball>());
        if (collision.transform.position.x >= gameObject.transform.position.x)
        {
            directions.Add((leftPos.position - collision.transform.position).normalized);
        }
        else
        {
            directions.Add((rightPos.position - collision.transform.position).normalized);
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    collision.GetComponent<Ball>().LeaveBottom();
    //    balls.Remove(collision.GetComponent<Ball>());
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        ball.Sprite.color = color;
        ball.Collider.sharedMaterial = null;
        ball.Rigidbody.velocity = collision.transform.position;
        ball.Trail.enabled = false;

        Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
        if(ball.isAtBottom == false)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        collision.gameObject.GetComponent<Ball>().isAtBottom = true;
        if (ball.isAtBottom)
        {
            rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            int ballIndex = GetIndex(ball);
            if (ballIndex >= 0)
            {
                rigidbody2D.AddForce(directions[ballIndex] * force, ForceMode2D.Impulse);
            }
            else
            {
                balls.Add(ball);
                Vector2 pushDirection;
                if (collision.transform.position.x >= gameObject.transform.position.x)
                {
                    pushDirection = (leftPos.position - collision.transform.position).normalized;
                }
                else
                {
                    pushDirection = (rightPos.position - collision.transform.position).normalized;
                }
                directions.Add(pushDirection);
                rigidbody2D.AddForce(pushDirection * force, ForceMode2D.Impulse);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        
    }
    private int GetIndex(Ball ball)
    {
        for(int i = 0; i < balls.Count; i++)
        {
            if(balls[i] == ball)
            {
                return i;
            }
        }
        return -1;
    }
    //private int GetClosestPosition(GameObject ball)
    //{
    //    float distance = 0;
    //    int i = 0;
    //    if(LeftOrRight(ball))
    //    {
    //        for(int index = 0; index < bottomPos.Count; index++)
    //        {
    //            if(index == 0 && bottomLeftPos[index].position.x <)
    //            {
    //                distance = (bottomLeftPos[index].position - ball.transform.position).magnitude;
    //                i = index;
    //            }
    //            else if(index > 0 && distance > (bottomLeftPos[index].position - ball.transform.position).magnitude)
    //            {
    //                distance = (bottomLeftPos[index].position - ball.transform.position).magnitude;
    //                i = index;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        for (int index = 0; index < bottomRightPos.Count; index++)
    //        {
    //            if (index == 0)
    //            {
    //                distance = (bottomRightPos[index].position - ball.transform.position).magnitude;
    //                i = index;
    //            }
    //            else if (index > 0 && distance > (bottomRightPos[index].position - ball.transform.position).magnitude)
    //            {
    //                distance = (bottomRightPos[index].position - ball.transform.position).magnitude;
    //                i = index;
    //            }
    //        }
    //    }
    //    return i;
    //}
    //private bool LeftOrRight(GameObject ball)
    //{
    //    if (ball.transform.position.x >= gameObject.transform.position.x)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}
    //private void MoveBall(GameObject ball)
    //{
    //    bool isLeftOrRight = LeftOrRight(ball);
    //    int index = GetClosestPosition(ball);
    //    BallMoveHandler(index, isLeftOrRight, ball.GetComponent<Rigidbody2D>());
    //}
    //private void BallMoveHandler(int index, bool leftOrRight, Rigidbody2D rigidbody2D)
    //{
    //    Sequence sequence = DOTween.Sequence();
    //    if (leftOrRight)
    //    {
    //        if(index == 0)
    //        {
    //            sequence.Append(rigidbody2D.DOMove(bottomLeftPos[0].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(bottomLeftPos[1].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(bottomLeftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            //Move ball up.
    //            .Append(rigidbody2D.DOMove(leftPos[0].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[1].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[3].position, 0.3f)).SetEase(Ease.Linear);
    //        }
    //        else if(index == 1)
    //        {
    //            sequence.Append(rigidbody2D.DOMove(bottomLeftPos[1].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(bottomLeftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            //Move ball up.
    //            .Append(rigidbody2D.DOMove(leftPos[0].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[1].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[3].position, 0.3f)).SetEase(Ease.Linear);
    //        }
    //        else if (index == 2)
    //        {
    //            sequence.Append(rigidbody2D.DOMove(bottomLeftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            //Move ball up.
    //            .Append(rigidbody2D.DOMove(leftPos[0].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[1].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[2].position, 0.3f)).SetEase(Ease.Linear)
    //            .Append(rigidbody2D.DOMove(leftPos[3].position, 0.3f)).SetEase(Ease.Linear);
    //        }
    //    }
    //}
    //public void PushBall(Ball ball)
    //{
    //    Vector2 pushDirection = new Vector2();
    //    if (ball.transform.position.x >= gameObject.transform.position.x)
    //    {
    //        pushDirection = new Vector2(-(forceDirection * 5), ball.transform.position.y);
    //    }
    //    else
    //    {
    //        pushDirection = new Vector2(forceDirection * 5, ball.transform.position.y);
    //    }
    //    ball.Rigidbody.AddForce(pushDirection, ForceMode2D.Impulse);
    //}
}
