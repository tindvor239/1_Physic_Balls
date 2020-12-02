using UnityEngine;

public class Item : MonoBehaviour
{
    protected static Vector2 position = Vector2.zero;
    protected Animator animator;
    protected bool isDoneMoving = true;
    #region Properties
    public bool IsDoneMoving { get => isDoneMoving; }
    public Animator Animator { get => animator; }
    #endregion
    protected void Awake()
    {
        position = transform.position;
        animator = GetComponent<Animator>();
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            GameManager.Instance.Score++;
            Destroy(gameObject);
        }
    }
    public void Moving(Vector2 target)
    {
        isDoneMoving = false;
        if (isDoneMoving == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 0.5f);
            if ((Vector2)transform.position == target)
            {
                position = transform.position;
                isDoneMoving = true;
            }
        }
    }
}
