using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Obstacle : Item
{
    [SerializeField]
    private Stat hp;
    [SerializeField]
    private Text hpUI;
    [SerializeField]
    private Geometry geometry;
    #region Properties
    public uint HP
    {
        get => hp.BaseStat;
        set
        {
            hp.SetBase(value);
            hpUI.text = hp.BaseStat.ToString();
        }
    }
    public bool IsGameOver { get; set; }
    public Geometry Geometry { get => geometry; }
    #endregion
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            HP--;
            GameManager.Instance.Score++;
            if (HP <= 0)
            {
                Pool pool = GameManager.Instance.PoolParty.GetPool("Particles Pool");
                CheckIsExtend(pool);
                BackToPool(pool);
            }
        }
    }
    private void CheckIsExtend(Pool pool)
    {
        ParticleSystem particle;
        if(pool.CanExtend)
        {
            particle = GameManager.Instance.PoolParty.CreateItem(pool, transform.position, 0, Spawner.Instance.transform).GetComponent<ParticleSystem>();
        }
        else
        {
            particle = pool.GetOutOfPool(transform.position).GetComponent<ParticleSystem>();
        }
        particle.Play();
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
public enum Geometry { circle, rectangle, triangle, hexagon}
