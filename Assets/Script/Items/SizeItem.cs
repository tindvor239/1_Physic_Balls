using UnityEngine;

public class SizeItem : AddItem
{
    [SerializeField]
    private uint maxSize = 6, minSize = 5;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.HitItemSound);
            if (collision.gameObject.transform.localScale.x >= maxSize)
            {
                collision.transform.localScale = new Vector2(minSize, minSize);
                AddBall(collision.gameObject);
                CreateFloatingText("Split");
            }
            else
            {
                collision.transform.localScale = new Vector2(maxSize, maxSize);
                CreateFloatingText("Double Size");
            }
        }
        BackToPool(GameManager.Instance.PoolParty.GetPool("Items Pool"));
    }
}
