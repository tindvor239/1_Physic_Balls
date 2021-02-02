﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Warning : Singleton<Warning>
{
    [SerializeField]
    private Image[] warnings = new Image[1];
    private Sequence sequence;
    #region Properties
    public Image[] Warnings { get => warnings; }
    #endregion
    private void Start()
    {
        //for(int index = 0; index < warnings.Length; index++)
        //{
        //    warnings[index].SetActive(false);
        //}
    }
    public void Blinking(int index, bool isActive)
    {
        if(isActive)
        {
            sequence = DOTween.Sequence();
            Image image = warnings[index].GetComponent<Image>();
            sequence.Append(image.DOFade(0, 0.1f).SetEase(Ease.Linear)).Append(image.DOFade(1f, 0.1f).SetEase(Ease.Linear));
            sequence.SetLoops(-1);
            isActive = false;
        }
    }
    public void StopBlinking()
    {
        sequence.Kill();
        warnings[0].DOFade(1f, 0.1f).SetEase(Ease.Linear);
    }
}
