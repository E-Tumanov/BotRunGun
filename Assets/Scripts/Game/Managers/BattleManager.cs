using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public class BattleManager : GModel
    {
        public IBoss Boss { get; private set; }
        public int PlayerHP { get; private set; }
        [SerializeField] FireColorBall boltPrefab;

        void Start ()
        {
            Eve.OnPlayerFinished.AddListener(this, x => OnPlayerFinished());
            Eve.OnBuyBullet.AddListener(this, OnBuyBullet);
            Eve.OnBossShot.AddListener (this, x => PlayerDamage (1000));
            PlayerHP = 1;

            MTask.Run (0.3f, () => {
                Boss = factoryBoss.CreateBoss ();
            });
        }

        void OnBuyBullet(BuyBullet x)
        {
            if (x.value > 0)
            {
                //boltManager.Change(x.value);
            }
        }

        // Стрельба босса
        void PlayerDamage(float val)
        {
            if (PlayerHP <= 0)
                return;

            PlayerHP -= (int)val;
            if (PlayerHP <= 0)
            {
                MTask.Run(0.2f, () => Eve.OnPlayerKilled.FireEvent(null));
                
                //  FIXIT камера сама может это понять
                CameraJitter.JitIt(1.0f); // потрясём камеру
            }
        }

        // Игрок доехал до финиша
        void OnPlayerFinished ()
        {
            // приехал пустой
            if (ballsManager.CurrCount == 0)
            {
                Eve.OnPlayerShot.FireEvent (new PlayerShot { isFirstMissShot = true, isMissShot = true });
            }
        }
    }
}