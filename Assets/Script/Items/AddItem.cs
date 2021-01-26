using UnityEngine;

public class AddItem : Item
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball" && collision.isTrigger == false)
        {
            GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.HitItemSound);
            CreateFloatingText("Extra Ball");
            BackToPool(GameManager.Instance.PoolParty.GetPool("Items Pool"));
            AddBall(collision.gameObject);
        }
    }

    protected void CreateFloatingText(string message)
    {
        Pool textPool = GameManager.Instance.PoolParty.GetPool("Floatings Pool");
        if (textPool != null)
        {
            GameObject newFloatingText = null;
            if (textPool.CanExtend)
            {
                newFloatingText = GameManager.Instance.PoolParty.CreateItem(textPool, transform.position, 0, transform.parent);
                Debug.Log("CanExtend");
                newFloatingText.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f);
            }
            else
            {
                GameObject getObject = GetFloatingText(textPool);
                newFloatingText = textPool.GetOutOfPool(getObject, new Vector2(transform.position.x, transform.position.y + 0.5f));
            }
            newFloatingText.GetComponent<FloatingText>().Floating();
            newFloatingText.GetComponent<TextMesh>().text = message;
        }
    }
    private GameObject GetFloatingText(in Pool textPool)
    {
        foreach (GameObject go in textPool.ObjectsPool)
        {
            if(go.activeInHierarchy == false)
            {
                return go;
            }
        }
        return null;
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
