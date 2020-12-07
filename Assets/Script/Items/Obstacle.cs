using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Obstacle : Item
{
    [SerializeField]
    private Text hitCount;

    #region Properties
    public uint HitCount
    {
        get => uint.Parse(hitCount.text);
        set => hitCount.text = value.ToString();
    }
    #endregion
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            HitCount--;
            GameManager.Instance.Score++;
            if (HitCount <= 0)
                Destroy(gameObject);
        }
    }

    public void Shaking()
    {
        animator.SetBool("isShaking", true);
    }
    public void StopShaking()
    {
        animator.SetBool("isShaking", false);
        transform.position = position;
    }
}
