using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected static Vector2 position = Vector2.zero;
    protected Animator animator;
    protected bool isDoneMoving = true;
    #region Properties
    public bool IsDoneMoving { get => isDoneMoving; }
    public Animator Animator { get => animator; }
    #endregion
    protected virtual void Awake()
    {
        position = transform.position;
        animator = GetComponent<Animator>();
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { }
    protected virtual void OnTriggerEnter2D(Collider2D collision) { }
    protected void BackToPool(Pool pool)
    {
        for (int row = 0; row < Spawner.instance.Obstacles.GetLength(0); row++)
        {
            for (int column = 0; column < Spawner.instance.Obstacles.GetLength(1); column++)
            {
                if (Spawner.instance.Obstacles[row, column] == this)
                {
                    Spawner.instance.Obstacles[row, column] = null;
                    pool.GetBackToPool(gameObject, GameManager.Instance.transform.position);
                    Debug.Log(string.Format("get {0} back to pool {1}", this.name, "Items Pool"));
                }
            }
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
