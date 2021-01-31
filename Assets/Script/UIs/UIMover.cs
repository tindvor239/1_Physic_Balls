using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : UIMenu
{
    // Start is called before the first frame update
    private void Start()
    {
        // GameManager.
        //images[0].rectTransform.DOMoveX(3, 1f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(images[0].rectTransform.DOLocalMoveX(sections[0].GetComponent<RectTransform>().localPosition.x, 2f).SetEase(Ease.Linear))
            .Append(images[0].rectTransform.DOLocalMoveX(sections[1].GetComponent<RectTransform>().localPosition.x, 2f).SetEase(Ease.Linear));
        sequence.SetLoops(-1);
    }
}
