using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace RBGame
{
    /// <summary>
    /// Реализует стрельбу, считает урон игроку, генерит FX/ Ага, заебись SRP %)))
    /// </summary>
    public class BattleManager : GModel
    {
        public IBoss Boss { get; private set; } // паблик, чтоб знать сколько патронов надо закупить
        public int PlayerHP { get; private set; }
        [SerializeField] FireColorBall boltPrefab;


        private void Start ()
        {
            eve.OnPlayerFinished.AddListener(this, x => OnPlayerFinished());
            eve.OnButtonPress.AddListener(this, OnPressFire);
            eve.OnBuyBullet.AddListener(this, OnBuyBullet);
            eve.OnBossShot.AddListener (this, x => PlayerDamage (1000));
            PlayerHP = 1;


            MTask.Run (0.3f, () => {
                Boss = factoryBoss.CreateBoss ();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnBuyBullet(BuyBullet x)
        {
            if (x.value > 0)
            {
                //boltManager.Change(x.value);
            }
        }


        /// <summary>
        /// Стрельба босса
        /// </summary>
        private void PlayerDamage(float val)
        {
            if (PlayerHP <= 0)
                return;

            PlayerHP -= (int)val;
            if (PlayerHP <= 0)
            {
                MTask.Run(0.2f, () => eve.OnPlayerKilled.FireEvent(new PlayerKilled()));
                
                //  FIXIT камера сама может это понять
                CameraJitter.JitIt(1.0f); // потрясём камеру
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnPressFire (ButtonPress x)
        {
            if (!(x.btn == GAME_BUTTON.MAIN && x.isPressed))
                return;
            
            if (Boss == null || Boss.CurrHP <= 0)
                return;

            if (!di.TripManager.IsFinished)
                return;

            if (PlayerHP <= 0)
                return;

            int colorNum = ballsManager.PopTopBall ();
            
            if (colorNum >= 0)
            {
                eve.OnPlayerShot.FireEvent (new PlayerShot { isMissShot = false });

                var bolt = Instantiate (boltPrefab);
                bolt.SetColorNum (colorNum);
                bolt.gameObject.SetActive (true);

                if (di.BallsManager.CurrCount == 0)
                {
                    // FIXIT Нужно подождать пока последний выстрел долетит и проверить боссХП
                    // игрок может аккуратно последний раз выстрелить, а система начнёт шуметь

                    eve.OnPlayerShot.FireEvent (new PlayerShot { isFirstMissShot = true, isMissShot = true });
                }
            }
            else
            {
                eve.OnPlayerShot.FireEvent (new PlayerShot { isMissShot = true });
            }
        }



        /// <summary>
        /// Обработчик. Игрок доехал до финиша
        /// </summary>
        private void OnPlayerFinished ()
        {

            // приехал пустой
            if (ballsManager.CurrCount == 0)
            {
                eve.OnPlayerShot.FireEvent (new PlayerShot { isFirstMissShot = true, isMissShot = true });
            }
        }

        /*
        /// <summary>
        /// Стрельба игрока
        /// </summary>
        private void OnPressFireOLD(ButtonPress x)
        {
            if (!(x.btn == GAME_BUTTON.MAIN && x.isPressed))
                return;
            
            if (Boss == null)
                return;
            
            if (Boss.HP <= 0)
                return;
            
            if (ballsManager.CollectedCount > 0)
            {
                eve.OnPlayerShot.FireEvent(new PlayerShot { isMissShot = false });
                int colorNum = ballsManager.PopTopBall ();

                
                // урон боссу
                if (Random.value < 0.2f) // ULTA
                {
                    Boss.Damage(colorNum, ConfDB.game.shootPrice, true);
                }
                else
                {
                    Boss.Damage(colorNum, ConfDB.game.shootPrice, false);
                }

                if (Boss.HP <= 0)
                {
                    Boss.Exploid(); // скрывает gameObject. не удаляет
                    eve.OnBossKilled.FireEvent(new BossKilled());
                }

                //  босс жив, а патроны кончились (событие)
                if (Boss.HP > 0 && ballsManager.CollectedCount <= 0)
                {
                    Boss.ActivateGunFireAfter (1.5f);
                    eve.OnPlayerShot.FireEvent (new PlayerShot { isMissShot = true, isFirstMissShot = true });
                }
            }
            else  //  Нет патронов
            {
                eve.OnPlayerShot.FireEvent(new PlayerShot { isMissShot = true });
            }
        }
        */

    }
}