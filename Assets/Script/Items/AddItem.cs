using UnityEngine;

public class AddItem : Item
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball" && collision.isTrigger == false)
        {
            BackToPool(GameManager.Instance.PoolParty.GetPool("Items Pool"));
            AddBall(collision.gameObject);
        }
    }
    protected virtual void AddBall(GameObject hitObject)
    {
        Ball newBall = Instantiate(GameManager.Instance.Level.BallPrefab, hitObject.transform.parent).GetComponent<Ball>();
        float gravity = GameManager.Instance.Gravity;
        newBall.Rigidbody.gravityScale = gravity;
        newBall.transform.position = hitObject.transform.position;
        foreach (Ball ball in GameManager.Instance.Level.Balls)
        {
            if (newBall != ball)
                Physics2D.IgnoreCollision(newBall.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
        }
        newBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(newBall.transform.position.x, newBall.transform.position.y) * 10);
    }
}
