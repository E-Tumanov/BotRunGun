using UIAnimatorCore;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace RBGame
{
    /// <summary>
    /// Виджет. Кол-во набранных патронов/монет
    /// </summary>
    public class Wdg_BulletStack : GModel
    {
        [SerializeField] Text coinCount;
        [SerializeField] Transform coinBar;
        [SerializeField] Transform coinLimitBar;
        [SerializeField] CanvasGroup canvas;
        [SerializeField] AnimationCurve popUpCurve;

        void Start()
        {
            if (G.isTutorEnable) // Fuuuuuu  HARDCODE
            {
                //  Через 11 сек. показать патроны
                canvas.alpha = 0;

                MTask.Run(this, 15, () =>
                {
                MTask.Run (this, 0, 0.5f, t =>
                 {
                     canvas.alpha = t;
                     transform.localScale = Vector3.one * popUpCurve.Evaluate (t);
                 }, 
                 () => { transform.localScale = Vector3.one; });
                });
            }

            
            //ballsManager.OnAddGroup += OnChangeCount;

            OnChangeCount(ballsManager.CurrCount, ballsManager.CurrCount);

            eve.OnPlayerShot.AddListener(this, x =>
            {
                if (x.isMissShot)
                {
                    GetComponent<UIAnimator>().ResetToStart();
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);
                }
            });
        }


        void OnChangeCount(int jcount, int prev)
        {
            coinCount.text = jcount.ToString(); // текст сейчас выключен 
            /*
            var sc = coinBar.localScale;
            if (SysBullet.totalCount == 0)
                sc.y = 1;
            else
                sc.y = (float)jcount / SysBullet.totalCount;
            coinBar.localScale = sc;
            */

            float sc;
            if (ballsManager.GroupCount == 0)
                sc = jcount;
            else
                sc = (float)jcount / ballsManager.GroupCount;

            coinBar.DOKill();
            {
                //fillDelay.localScale = new Vector3(currHP / boss.maxHP, 1, 1);
                coinBar.DOScaleY(sc, 0.15f);
            }

            /*
            sc = coinLimitBar.localScale;
            if (G.isFinished) // после фининиша (когда стреляем) уже не надо показывать лимит
                sc.y = 0;
            else
                sc.y = G.bossHP / (1.0f + SysCoin.totalCount);
            */
            //coinLimitBar.localScale = sc;
        }
    }
}

/*
- Ну смотри. Тестирование в полной жопе. Без инициации SysCoins и G.GameEvent - сильная связь
Хочу, чтоб был нормальный способ получать данные об игре(DBASE) и событиях (GM_EVENTS)

- Хотел? Получай )) DI в полный рост
*/
