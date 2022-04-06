using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    public class PlayerFight : GModel
    {
        [SerializeField] FireColorBall boltPrefab;

        Animator anima;
        bool playerKilled;
        bool stopped2Fight;
        float shootTime;

        void Awake ()
        {
            anima = GetComponent<Animator> ();
        }


        void Start ()
        {
            if (GameContext.Instance == null)
            {
                enabled = false;
                return;
            }

            cx.GamePad.OnButtonTouch += GamePad_OnButtonTouch;

            Eve.OnPlayerStopped2Fight.AddListener (this, x => {
                stopped2Fight = true;
                anima.CrossFade ("idle_throw", 0.05f);
            });

            Eve.OnPlayerKilled.AddListener (this, x => {
                anima.CrossFade ("stumble_backwards", 0.05f);
                playerKilled = true;
            });

            Eve.OnGameOver.AddListener (this, gameResult => {
                if (gameResult)
                {
                    anima.Play ("dancing_swing");
                    stopped2Fight = false;
                }
            });
        }

        bool clickTrap;
        private void GamePad_OnButtonTouch (GAME_BUTTON btn, bool isPressed)
        {
            //if (isPressed == false && clickTrap == true)
            if (isPressed)
            {
                if (Time.unscaledTime > shootTime)
                {
                    shootTime = Time.unscaledTime + 0.2f;
                    MTask.Run (0.03f, () => OnPressFire ());
                }
            }
            clickTrap = isPressed;
        }


        //  стрельба игрока
        bool leftHandFight;
        void OnPressFire ()
        {
            if (playerKilled)
                return;

            if (!stopped2Fight)
                return;

            if (!cx.TripManager.IsFinished)
                return;

            if (leftHandFight)
            {
                anima.CrossFade ("throw_left", 0.00f, 0, 0);
            }
            else
            {
                anima.CrossFade ("throw_right", 0.00f, 0, 0);
            }
            leftHandFight = !leftHandFight;

            int colorNum = ballsManager.PopTopBall ();

            if (colorNum >= 0)
            {
                Eve.OnPlayerShot.FireEvent (new PlayerShot { isMissShot = false });

                var bolt = Instantiate (boltPrefab);
                bolt.SetColorNum (colorNum);
                bolt.gameObject.SetActive (true);

                if (cx.BallsManager.CurrCount == 0)
                {
                    // FIXIT Нужно подождать пока последний выстрел долетит и проверить боссХП
                    // игрок может аккуратно последний раз выстрелить, а система начнёт шуметь

                    Eve.OnPlayerShot.FireEvent (new PlayerShot { isFirstMissShot = true, isMissShot = true });
                }
            }
            else
            {
                Eve.OnPlayerShot.FireEvent (new PlayerShot { isMissShot = true });
            }
        }
    }
}
