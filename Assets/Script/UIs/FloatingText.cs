using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    private TextMesh textMesh;
    private float alpha = 1.0f;
    private static float duration = 1.0f;
    private float destroyTime = 0f;
    // Start is called before the first frame update
    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }
    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            destroyTime += Time.deltaTime;
            alpha -= Time.deltaTime;
            Color color = textMesh.color;
            textMesh.color = ConvertColorToColor32(new Color(color.r, color.g, color.b, alpha));
            if(destroyTime >= duration)
            {
                GameManager.Instance.PoolParty.GetPool("Floatings Pool").GetBackToPool(gameObject, GameManager.Instance.transform.position);
                Debug.Log("GetBackToPool");
            }
        }
    }
    public void Floating()
    {
        destroyTime = 0.0f;
        alpha = 1.0f;
        transform.DOMoveY(transform.position.y + 1.0f, duration).SetEase(Ease.Linear);
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
