using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Obstacle : Item
{
    [SerializeField]
    private Stat hp;
    [SerializeField]
    private Transform mainImage;
    [SerializeField]
    private Text hpUI;
    [SerializeField]
    protected Geometry geometry;
    [SerializeField]
    private SpriteRenderer background;
    #region Properties
    public int HP
    {
        get => hp.BaseStat;
        set
        {
            hp.SetBase(value);
            hpUI.text = hp.BaseStat.ToString();
        }
    }
    public byte HitCount { get; set; }
    public SpriteRenderer Background { get => background; }
    public Transform MainImage { get => mainImage; }
    public Geometry Geometry { get => geometry; }
    #endregion
    public void OnChildCollisionEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            HP--;
            GameManager.Instance.Score++;
            OnHit();
        }
    }

    public void OnHit()
    {
        GameManager.Instance.SetSpriteColor(this);
        ChangeColorOnHit();
        if (HP <= 0)
        {
            Pool pool = GameManager.Instance.PoolParty.GetPool("Particles Pool");
            SpawnParticle(pool);
            BackToPool(pool);
            HitCount = 0;
        }
    }
    private void SpawnParticle(Pool pool)
    {
        ParticleSystem particle = null;
        if(pool.CanExtend)
        {
            particle = GameManager.Instance.PoolParty.CreateItem(pool, transform.position, 0, Spawner.Instance.transform).GetComponent<ParticleSystem>();
        }
        else
        {
            // check geometry spawn the correct object!
            foreach(GameObject gameObject in pool.ObjectsPool)
            {
                if(gameObject.activeInHierarchy == false)
                {
                    particle = pool.GetOutOfPool(gameObject, transform.position).GetComponent<ParticleSystem>();
                }
            }
        }
        particle.textureSheetAnimation.SetSprite(0, mainImage.GetComponent<SpriteRenderer>().sprite);
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
    private void ChangeColorOnHit()
    {
        Color baseColor = background.color;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(background.DOColor(ConvertColorToColor32(new Color(1, 1, 1, baseColor.a)), 0.1f)).Append(background.DOColor(ConvertColorToColor32(baseColor), 0.1f));
    }
    private Color32 ConvertColorToColor32(Color color)
    {
        Color32 color32 = new Color32();
        color32.r = (byte)(color.r * 255);
        color32.g = (byte)(color.g * 255);
        color32.b = (byte)(color.b * 255);
        color32.a = (byte)(color.a * 255);
        return color32;
    }
}
public enum Geometry { circle, cube, isoscelesTriangle, rightTriangle, pentagon}
