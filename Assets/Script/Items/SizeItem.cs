using UnityEngine;

public class SizeItem : AddItem
{
    [SerializeField]
    private float maxSize = 3f, minSize = 2.5f;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            if (GameManager.Instance.Mute == false)
            {
                GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.HitItemSound);
            }
            if (collision.gameObject.transform.localScale.x >= maxSize)
            {
                collision.transform.localScale = new Vector2(minSize, minSize);
                AddBall(collision.gameObject);
                CreateFloatingText("Split");
            }
            else
            {
                collision.transform.localScale = new Vector2(maxSize, maxSize);
                CreateFloatingText("Extra Size");
            }
        }
        BackToPool(GameManager.Instance.PoolParty.GetPool("Items Pool"));
    }
}
