using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Создаёт окна в игре. По событиям
    /// </summary>
    public class GameUIManager : GModel
    {
        void Start ()
        {
            Eve.OnPlayerShot.AddListener (this, OnPlayerShot);
            Eve.OnPlayerKilled.AddListener (this, x => OnDefeat (ConfDB.game.ads_inter_p_killed));
            Eve.OnPlayerImpact.AddListener (this, x => OnDefeat (ConfDB.game.ads_inter_p_impact));
            Eve.OnGameOver.AddListener (this, x => {
                if (x)
                    OnVictory ();
            });
        }

        // Победа в раунде
        void OnVictory ()
        {
            // окно победы/ реклама
            MTask.Run (ConfDB.game.tm_ow_victory, () => {
                if (Random.value < ConfDB.game.ads_inter_p_victory)
                {
                    ADS.OpenInter (() => UIManager.OpenWin (WINDOW.GAME_VICTORY, null));
                }
                else
                {
                    UIManager.OpenWin (WINDOW.GAME_VICTORY, null);
                }
            });
        }


        // Обработчик. Первый "пустой" выстрел. Пора рекламу посмотреть
        void OnPlayerShot (PlayerShot ps)
        {
            if (ps.isFirstMissShot)
            {
#if (GGL_REWARD)
                UIManager.OpenWin(WINDOW.BUY_BULLET, null);
#endif
            }
        }

        void OnDefeat (float pauseTime)
        {
            UIManager.OpenWin (WINDOW.__end, null);

            MTask.Run (ConfDB.game.tm_ow_impact, () => {
                if (Random.value < pauseTime) // вероятность
                {
                    ADS.OpenInter (() => UIManager.OpenWin (WINDOW.GAME_DEFEAT, null));
                }
                else
                {
                    UIManager.OpenWin (WINDOW.GAME_DEFEAT, null);
                }
            });
        }

        /*
        // Обработчик. Игрока убили
        void OnPlayerKilled ()
        {
            UIManager.OpenWin (WINDOW.__end, null);

            MTask.Run (ConfDB.game.tm_ow_impact, () => {
                if (Random.value < ConfDB.game.ads_inter_p_killed) // вероятность
                {
                    ADS.OpenInter (() => UIManager.OpenWin (WINDOW.GAME_DEFEAT, null));
                }
                else
                {
                    UIManager.OpenWin (WINDOW.GAME_DEFEAT, null);
                }
            });
        }


        // Обработчик. Игрок врезался
        void OnPlayerImpact ()
        {
            UIManager.OpenWin (WINDOW.__end, null);

            MTask.Run (ConfDB.game.tm_ow_impact, () => {
                if (Random.value < ConfDB.game.ads_inter_p_impact) // вероятность
                {
                    ADS.OpenInter (() => UIManager.OpenWin (WINDOW.GAME_DEFEAT, null));
                }
                else
                {
                    UIManager.OpenWin (WINDOW.GAME_DEFEAT, null);
                }
            });
        }*/
    }
}