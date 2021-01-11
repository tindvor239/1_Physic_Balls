using UnityEngine;
using DG.Tweening;

public class Warning : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] warnings;
    #region Singleton
    public static Warning instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    #region Properties
    public SpriteRenderer[] Warnings { get => warnings; }
    #endregion
    public void Blinking(SpriteRenderer spriteRenderer, bool isActive)
    {
        if(isActive)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOFade(0, 0.1f).SetEase(Ease.Linear)).Append(spriteRenderer.DOFade(1f, 0.1f).SetEase(Ease.Linear));
            sequence.SetLoops(3);
            isActive = false;
        }
    }
}
