using UnityEngine;

public class SizeItem : AddItem
{
    [SerializeField]
    private uint maxSize = 6, minSize = 5;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            if (collision.gameObject.transform.localScale.x >= maxSize)
            {
                collision.transform.localScale = new Vector2(minSize, minSize);
                AddBall(collision.gameObject);
            }
            else
                collision.transform.localScale = new Vector2(maxSize, maxSize);
        }
        Destroy(gameObject);
    }
}
