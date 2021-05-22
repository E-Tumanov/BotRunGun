using UnityEngine;

namespace RBGame
{



    /// <summary>
    /// Общее
    /// </summary>
    public partial class PlayerModel2 : GModel, IPlayerModel
    {
        public Vector3 position => ppos;
        public float levelSpeed { get; private set; }
        public float wheelYaw { get; private set; }
        public float bodyRoll { get; private set; }
        public float stoppedPos { get; private set; }

        public void SetButton (bool isPressed) { }
        public void SetLeftButton (bool isPressed) { }
        public void SetRightButton (bool isPressed) { }
        public void SetStoppedDist (float stoppedDist) { }
        public void SetPos (Vector3 newPos) { }
        public void Stop () { }

        Vector3 ppos;
        public void Run () { }
        void Update ()
        { 
        }
    }


    /// <summary>
    /// Общее
    /// </summary>
    public partial class PlayerModel : GModel, IPlayerModel
    {
        public Vector3 position => ppos;
        public float levelSpeed { get; private set; }
        public float wheelYaw { get; private set; }
        public float bodyRoll { get; private set; }
        public float stoppedPos { get; private set; }

        BotInfo botInfo;

        Vector3 fpos; // точка следует за ppos. Вектор наклона/ Плохая идея. На малых значениях каша
        Vector3 ppos;
        Vector3 prevPos;
        bool isFreeze;

        public void Run () { }

        private void Start ()
        {
            botInfo = ConfDB.bot;

            Eve.OnButtonPress.AddListener (this, OnButtonsPressed);
            Eve.OnPlayerImpact.AddListener (this, x => OnImpactOrKill ());
            Eve.OnPlayerKilled.AddListener (this, x => OnImpactOrKill ());

            // немного каша
            Eve.OnPlayerFinished.AddListener (this, x => SetStoppedDist (4));
        }

        private void Update ()
        {
            if (G.isPause)
                return;

            if (G.deltaTime > 0.1f)
                return;

            if (isFreeze)
                return;

            prevPos = ppos;

            SolveX (G.deltaTime);
            SolveY (G.deltaTime);

            ppos.y = roadMgr == null ? 0 : di.GetUpOffset (ppos.z);
        }

        private void OnImpactOrKill ()
        {
            isFreeze = true;
        }

        private void OnButtonsPressed (ButtonPress x)
        {
            switch (x.btn)
            {
                case GAME_BUTTON.MAIN: SetButton (x.isPressed); break;
                case GAME_BUTTON.LEFT: SetLeftButton (x.isPressed); break;
                case GAME_BUTTON.RIGHT: SetRightButton (x.isPressed); break;
            }
        }

        public void SetStoppedDist (float stoppedDist)
        {
            this.stoppedPos = ppos.z + stoppedDist;

            MTask.Run (this, 1, 2f, t => {
                speedMod_FinishMul01 = 1 - t;
            }, () => {
                speedMod_FinishMul01 = 0;
                isFreeze = true;
            });
        }

        public void Stop ()
        {
            StopY ();
            StopX ();
        }

        public void SetPos (Vector3 newPos)
        {
            ppos = prevPos = fpos = newPos;
            wheelYaw = 0;
            bodyRoll = 0;
        }
    }


    /// <summary>
    /// Движение по Вертикали 
    /// </summary>
    public partial class PlayerModel
    {
        float speedMod_FinishMul01 = 1;
        float mSpeedBase01;

        private void StopY ()
        {
            levelSpeed = 0;
            mSpeedBase01 = 0;
        }

        private void SolveY (float dt)
        {
            if (isFreeze)
                return;

            // набор скорости вначале
            mSpeedBase01 += ConfDB.bot.BaseAccel * dt;
            mSpeedBase01 = Mathf.Clamp01 (mSpeedBase01);

            float res = botInfo.BaseSpeed * mSpeedBase01 + ConfDB.bot.LongTapVSpeedAdd * longSpeedCurved01;
            levelSpeed += botInfo.ShiftTension * (res - levelSpeed) * dt;


            // speedMod_FinishMul01 = stoppedDist > 0 ? Mathf.Clamp01 ((stoppedPos - ppos.z) / stoppedDist) : 1;

            // торможение
            levelSpeed *= speedMod_FinishMul01;

            ppos.z += levelSpeed * dt;
        }
    }


    /// <summary>
    /// Движение по Горизонтали
    /// </summary>
    public partial class PlayerModel
    {
        bool mainButtonPressed;
        bool leftButtonPressed;
        bool rightButtonPressed;

        //  VARS
        float shiftSpeed;
        float longSpeedCurved01;
        float longSpeedMUL01;

        float shiftDir;
        float shiftTarget;

        bool bPressed = false;

        public void SetButton (bool isPressed)
        {
            mainButtonPressed = isPressed;
        }

        public void SetLeftButton (bool isPressed)
        {
            leftButtonPressed = isPressed;
            if (isPressed)
            {
                shiftDir = -1;
            }
            bPressed = rightButtonPressed || leftButtonPressed;
            if (!bPressed)
                shiftDir = 0;
        }

        public void SetRightButton (bool isPressed)
        {
            rightButtonPressed = isPressed;
            if (isPressed)
            {
                shiftDir = +1;
            }
            bPressed = rightButtonPressed || leftButtonPressed;
            if (!bPressed)
                shiftDir = 0;
        }

        private void StopX ()
        {
            shiftSpeed = longSpeedCurved01 = shiftDir = 0;
        }

        /// <summary>
        /// Расчёт скорости и положения по Горизонтали
        /// </summary>
        private void SolveX (float dt)
        {
            shiftTarget += botInfo.ShiftTension * (shiftDir - shiftTarget) * dt;

            if (bPressed || mainButtonPressed)
            {
                longSpeedMUL01 += botInfo.LongTapAccel * dt;
            }
            else
            {
                longSpeedMUL01 -= botInfo.LongTapDeccel * dt;
            }
            longSpeedMUL01 = Mathf.Clamp01 (longSpeedMUL01);

            // форма кривой
            var useSin = false;
            longSpeedCurved01 = useSin ? SinLerp (longSpeedMUL01) : Mathf.Pow (longSpeedMUL01, botInfo.LongTapPow);

            shiftSpeed = botInfo.ShiftSpeed;
            shiftSpeed += botInfo.LongTapHSpeedAdd * longSpeedCurved01;
            shiftSpeed *= shiftTarget;

            // торможение. 
            shiftSpeed *= speedMod_FinishMul01;

            // Итого. Перемещение
            ppos.x += shiftSpeed * dt;

            if (ppos.x < stage.LeftBorder)
            {
                ppos.x = stage.LeftBorder;
                shiftTarget = 0;
            }

            if (ppos.x > stage.RightBorder)
            {
                ppos.x = stage.RightBorder;
                shiftTarget = 0;
            }

            //  наклон тела и поворот колеса
            {
                fpos = Vector3.Lerp (fpos, ppos, 10 * dt); // follow point
                bodyRoll = -35 * (fpos.x - ppos.x);
                wheelYaw = 1.5f * bodyRoll;
            }
        }

        private float SinLerp (float t)
        {
            return 0.5f + Mathf.Sin (3.1415f * (t - 0.5f)) * 0.5f;
        }
    }
}