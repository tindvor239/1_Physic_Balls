using UnityEngine;

public class AddItem : Item
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.localScale = new Vector2(collision.transform.localScale.x, collision.transform.localScale.x);
        GameObject newBall = Instantiate(collision.gameObject, collision.transform.parent);
        newBall.transform.position = collision.transform.position;
        GameManager.Instance.Balls.Add(newBall);
        foreach (GameObject ball in GameManager.Instance.Balls)
        {
            if (newBall != ball)
                Physics2D.IgnoreCollision(newBall.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
        }
        Destroy(gameObject);
    }
}
