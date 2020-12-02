using UnityEngine;

public class SizeItem : Item
{
    [SerializeField]
    private uint maxSize = 6, minSize = 5;
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            if(collision.gameObject.transform.localScale.x >= maxSize)
            {
                collision.transform.localScale = new Vector2(minSize, minSize);
                GameObject newBall = Instantiate(collision.gameObject, collision.transform.parent);
                newBall.transform.position = collision.transform.position;
                foreach (GameObject ball in GameManager.Instance.Balls)
                {
                    if (newBall != ball)
                        Physics2D.IgnoreCollision(newBall.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
                }
                GameManager.Instance.Balls.Add(newBall);
            }
            else
                collision.transform.localScale = new Vector2(maxSize, maxSize);
        }
        Destroy(gameObject);
    }
}
