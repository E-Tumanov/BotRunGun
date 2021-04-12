using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Создаёт окна в игре. По событиям
    /// </summary>
    public class WinGameManager : GModel
    {
        private void Start()
        {
            eve.OnPlayerShot.AddListener(this,OnPlayerShot);
            eve.OnBossKilled.AddListener(this, _ => OnVictory());
            eve.OnPlayerFinished.AddListener(this, _ => OnPlayerFinished());
            eve.OnPlayerKilled.AddListener(this, _ => OnPlayerKilled());
            eve.OnPlayerImpact.AddListener(this, _ =>OnPlayerImpact());
        }


        /// <summary>
        /// Обработчик. Игрок доехал до финиша
        /// </summary>
        private void OnPlayerFinished()
        {
/*
            //  Приехал, а патронов нет
            if (ballsManager.CurrCount == 0)
            {
                MTask.Run(2.0f, () =>
                {
#if (GGL_REWARD)
                    UIManager.OpenWin(WINDOW.BUY_BULLET, null);
#endif
                });
            }
*/
        }


        /// <summary>
        /// Обработчик. Босс уничтожен
        /// </summary>
        private void OnVictory()
        {
            // окно победы/ реклама
            MTask.Run(ConfDB.game.tm_ow_victory, () =>
            {
                if (Random.value < ConfDB.game.ads_inter_p_victory)
                {
                    ADS.OpenInter(() => UIManager.OpenWin(WINDOW.GAME_VICTORY, null));
                }
                else
                {
                    UIManager.OpenWin(WINDOW.GAME_VICTORY, null);
                }
            });
        }


        /// <summary>
        /// Обработчик. Первый "пустой" выстрел. Пора рекламу посмотреть
        /// </summary>
        private void OnPlayerShot(PlayerShot ps)
        {
            if (ps.isFirstMissShot)
            {
#if (GGL_REWARD)
                UIManager.OpenWin(WINDOW.BUY_BULLET, null);
#endif
            }
        }


        /// <summary>
        /// Обработчик. Игрока убили
        /// </summary>
        private void OnPlayerKilled()
        {
            UIManager.OpenWin(WINDOW.__end, null);

            MTask.Run(ConfDB.game.tm_ow_impact, () =>
            {
                if (Random.value < ConfDB.game.ads_inter_p_killed) // вероятность
                {
                    ADS.OpenInter(() => UIManager.OpenWin(WINDOW.GAME_DEFEAT, null));
                }
                else
                {
                    UIManager.OpenWin(WINDOW.GAME_DEFEAT, null);
                }
            });
        }


        /// <summary>
        /// Обработчик. Игрок врезался
        /// </summary>
        private void OnPlayerImpact()
        {
            UIManager.OpenWin(WINDOW.__end, null);
            
            MTask.Run(ConfDB.game.tm_ow_impact, () =>
            {
                if (Random.value < ConfDB.game.ads_inter_p_impact) // вероятность
                {
                    ADS.OpenInter(() => UIManager.OpenWin(WINDOW.GAME_DEFEAT, null));
                }
                else
                {
                    UIManager.OpenWin(WINDOW.GAME_DEFEAT, null);
                }
            });
        }
    }
}