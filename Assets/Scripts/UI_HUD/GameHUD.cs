using DG.Tweening;
using System.Collections;
using UIAnimatorCore;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    public class GameHUD : GModel
    {
        [SerializeField] Transform textPanel;
        [SerializeField] Text textMsg;

        Sequence msgSeq = null;

        public void Append(Transform wdg)
        {
            wdg.SetParent(transform, false);
        }

        public void DisplayMsg(string msg, float ttl)
        {
            ShowMsg(msg, ttl);
        }

        private void Start()
        {           
            Eve.OnPlayerKilled.AddListener(this, x =>
            {
                HideMsg();
                ShowMsg(Locale.Get("defeat"));
            });

            Eve.OnBossKilled.AddListener(this, x =>
            {
                HideMsg();
                /*
                DOTween.Sequence().AppendInterval(0.5f).OnComplete(() =>
                {
                    ShowMsg(Locale.Get("victory"), 200);
                }).SetLink(gameObject);
                */
            });

            Eve.OnBuyBullet.AddListener(this, x =>
            {
                HideMsg();
                if (x.value > 0)
                    ShowMsg(Locale.Get("finish_him"));
            });

            if (G.isTutorEnable)
            {
                Eve.OnPlayerFinished.AddListener (this, x =>
                 {
                     HideMsg ();
                     DOTween.Sequence ().AppendInterval (0.5f).OnComplete (() =>
                   {
                         ShowMsg (Locale.Get ("tap_faster"));
                     }).SetLink (gameObject);
                 });
            }
        }

        private void ShowMsg(string msg, float ttl = 3)
        {
            msgSeq.Kill();

            if (msg == "")
                Debug.LogError("GameHUD.ShowMsg> Empty msg");

            textPanel.GetComponent<UIAnimator>().ResetToStart();
            textPanel.gameObject.SetActive(false);
            textPanel.gameObject.SetActive(true);
            textMsg.text = msg;

            msgSeq = DOTween.Sequence().AppendInterval(ttl).OnComplete(() =>
            {
                textPanel.gameObject.SetActive(false);
                msgSeq = null;
            }).SetLink(gameObject);
        }

        private void HideMsg()
        {
            msgSeq?.Kill();
            msgSeq = null;
            textPanel.gameObject.SetActive(false);
        }
    }
}
