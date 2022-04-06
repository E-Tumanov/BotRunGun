using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    /// <summary>
    /// Плашка покупки патронов
    /// </summary>
    public class Win_BuyBullet : GModel, IWindow
    {
        [SerializeField] Button buy;
        [SerializeField] Text bulletCount;

        private void Start()
        {
#if (ZX_RELEASE)
            //  В релизе, если нет рекламы, то нет монет
            // Сразу выходим
            if (!ADS.IsAvaliableReward())
            {
                Eve.OnBuyBullet.FireEvent (new BuyBullet { isCanceled = true });
                UIManager.OpenWin(WINDOW.__end, null);
                return;
            }
#endif

            int bulletSum = 2 * Mathf.CeilToInt(battleMgr.Boss.CurrHP / 10.0f) * 10;

            bulletCount.text = "+" + bulletSum.ToString();

            buy.onClick.AddListener(() =>
            {
                UIManager.OpenWin(WINDOW.__end, null);

                Eve.OnBuyBullet.FireEvent(new BuyBullet { isStarted = true });

                ADS.OpenReward(
                    () => Eve.OnBuyBullet.FireEvent(new BuyBullet { value = bulletSum }),
                    () => Eve.OnBuyBullet.FireEvent(new BuyBullet { isCanceled = true })
                );
                /*
                if (AdsRevardVideo.inst && AdsRevardVideo.inst.IsLoadRewardVideo())
                {
                    // досмотрел и закупился
                    AdsRevardVideo.inst.OnEarnedReward += () =>
                    {
                        gev.Send(new Event.BuyBullet { value = bulletSum });
                    };

                    // не вытерпел, отмена
                    AdsRevardVideo.inst.OnForceCloseReward += () =>
                    {
                        gev.Send(new Event.BuyBullet { isCanceled = true });
                    };

                    AdsRevardVideo.inst.PlayRewardVideo();
                }
                else
                {
                    gev.Send(new Event.BuyBullet { value = bulletSum });
                }*/
            });
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }
}