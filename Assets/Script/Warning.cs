using UnityEngine;
using DG.Tweening;

public class Warning : Singleton<Warning>
{
    [SerializeField]
    private GameObject[] warnings;
    #region Properties
    public GameObject[] Warnings { get => warnings; }
    #endregion
    private void Start()
    {
        for(int index = 0; index < warnings.Length; index++)
        {
            warnings[index].SetActive(false);
        }
    }
    public void Blinking(int index, bool isActive)
    {
        if(isActive)
        {
            Sequence sequence = DOTween.Sequence();
            SpriteRenderer spriteRenderer = warnings[index].GetComponent<SpriteRenderer>();
            sequence.Append(spriteRenderer.DOFade(0, 0.1f).SetEase(Ease.Linear)).Append(spriteRenderer.DOFade(1f, 0.1f).SetEase(Ease.Linear));
            sequence.SetLoops(3);
            isActive = false;
        }
    }
}
