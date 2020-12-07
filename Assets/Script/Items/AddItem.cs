using UnityEngine;

public class AddItem : Item
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            AddBall(collision.gameObject);
            Destroy(gameObject);
        }
    }
    protected virtual void AddBall(GameObject hitObject)
    {
        GameObject newBall = Instantiate(hitObject, hitObject.transform.parent);
        newBall.transform.position = hitObject.transform.position;
        GameManager.Instance.Balls.Add(newBall);
        foreach (GameObject ball in GameManager.Instance.Balls)
        {
            if (newBall != ball)
                Physics2D.IgnoreCollision(newBall.GetComponent<Collider2D>(), ball.GetComponent<Collider2D>(), true);
        }
        newBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(newBall.transform.position.x, newBall.transform.position.y) * 10);
    }
}
