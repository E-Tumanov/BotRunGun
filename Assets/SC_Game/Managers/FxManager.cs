
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// FxManager
    /// </summary>
    public class FxManager : GModel
    {
        [SerializeField] Transform fx_confetti_shower_rainbow;
        [SerializeField] Transform fx_bengal_GROUP;

        private void Start()
        {
            eve.OnBossKilled.AddListener(this, _ => { OnBossKilled(); });
            eve.OnPlayerShot.AddListener(this, OnPlayerShot);            
        }


        /// <summary>
        /// Обработчик. Стрельба игрока
        /// </summary>
        private void OnPlayerShot(PlayerShot x)
        {
            if (x.isMissShot)
                return;

            SoundManager.shoot.pitch = Random.Range(0.7f, 1);
            SoundManager.shoot.volume = Random.Range(0.7f, 1);
            SoundManager.shoot.Play();
        }


        /// <summary>
        /// Обработчик. Босс уничтожен
        /// </summary>
        private void OnBossKilled()
        {
            MTask.Run(1, () => VibratorWrapper.Vibrate(350));// жужим
            SoundManager.winner.Play();// аплодисменты

            //  И конфетииишки!
            /*
            var fx = Instantiate(fx_confetti_shower_rainbow);
            var sz = bot.stoppedPos - 10;
            fx.position = new Vector3(0, di.GetUpOffset(sz) + 11, sz);
            */

            var fx = Instantiate(fx_bengal_GROUP);
            var sz = bot.position.z;
            fx.position = new Vector3(0, di.GetUpOffset (sz), sz);
        }
    }
}