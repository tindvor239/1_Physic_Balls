using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Focusing : Singleton<Focusing>
{
    [SerializeField]
    private Text countDown;
    [SerializeField]
    private GameObject continueButton;
    private float startTimer = 11f;
    private float timer = 11f;
    // Start is called before the first frame update
    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.8f)).SetEase(Ease.Linear).Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.8f)).SetEase(Ease.Linear);
        sequence.SetLoops(-1);
    }
    private void OnEnable()
    {

        continueButton.SetActive(false);
        gameObject.SetActive(true);
        countDown.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        timer = startTimer;
    }
    // Update is called once per frame
    void Update()
    {
        if(isActiveAndEnabled)
        {
            TimeCount();
            if(timer <= 0)
            {
                continueButton.SetActive(true);
                gameObject.SetActive(false);
                countDown.gameObject.SetActive(false);
            }
        }
    }
    private void TimeCount()
    {
        timer -= Time.deltaTime;
        string seconds = Mathf.RoundToInt(timer % 60).ToString();
        countDown.text = seconds;
    }

    public void OnClickWatch()
    {
        //Show Watch video on.
        if (ZenSDK.instance.IsVideoRewardReady())
        {
            ZenSDK.instance.ShowVideoReward((bool isSuccess) =>
            {
                //Revive.
            });
        }
        Revive();
        GameManager.Instance.Continue();
    }
    private void Revive()
    {
        foreach(GameObject gameObject in Gameover.Instance.GameOverObjects)
        {
            GameManager.Instance.PoolParty.GetPool("Obstacles Pool").GetBackToPool(gameObject.transform.parent.gameObject, GameManager.Instance.transform.position);
        }
        Gameover.Instance.GameOverObjects.Clear();
    }
}
