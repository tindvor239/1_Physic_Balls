using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected static Vector2 position = Vector2.zero;
    protected Animator animator;
    #region Properties
    public Animator Animator { get => animator; }
    #endregion
    protected virtual void Awake()
    {
        position = transform.position;
        animator = GetComponent<Animator>();
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { }
    protected virtual void OnTriggerEnter2D(Collider2D collision) { }
    public void BackToPool(Pool pool)
    {
        for (int row = 0; row < Spawner.Instance.Obstacles.rows.Count; row++)
        {
            for (int column = 0; column < Spawner.Instance.Obstacles.rows[0].columns.Count; column++)
            {
                if (Spawner.Instance.Obstacles.rows[row].columns[column] == this)
                {
                    Spawner.Instance.Obstacles.rows[row].columns[column] = null;
                    pool.GetBackToPool(gameObject, GameManager.Instance.transform.position);
                }
            }
        }
    }
}
