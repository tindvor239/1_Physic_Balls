using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Led : MonoBehaviour
{
    private Image image;
    private static float blinkingTimeDelay1 = 1.2f, blinkingTimeDelay2 = 0.3f;
    private float blinkingTime;
    private int count = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
        blinkingTime = blinkingTimeDelay1;
    }
    private void Update()
    {
        blinkingTime -= Time.deltaTime;
        if(blinkingTime <= 0)
        {
            image.enabled = !image.enabled;
            count++;
            if(count >= 4)
            {
                count = 0;
            }
            if(count > 0)
            {
                blinkingTime = blinkingTimeDelay2;
            }
            else
            {
                blinkingTime = blinkingTimeDelay1;
            }
        }
    }
}
