﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Модель движения игрока
    /// </summary>
    public interface IPlayerModel
    {
        Vector3 position { get; }
        float levelSpeed { get; }
        float wheelYaw { get; }
        float bodyRoll { get; }
        float stoppedPos { get; }

        void SetStoppedDist (float stoppedDist);
        void SetPos (Vector3 newPos);
        void Run ();
        void Stop ();
    }

    public partial class PlayerMover : GModel, IPlayerModel
    {
        public Vector3 position => ppos;

        public float levelSpeed { get; set; }
        public float wheelYaw { get; set; }
        public float bodyRoll { get; set; }
        public float stoppedPos { get; set; }

        Animator anima; // это больше для вьюхи

        BotInfo botInfo;
        bool isFreez;
        Vector3 ppos;

        private void Awake ()
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

            botInfo = ConfDB.bot;

            di.GamePad.OnButtonTouchVel += OnInput;

            Eve.OnPlayerFinished.AddListener (this, x => SetStoppedDist (4));
            Eve.OnPlayerImpact.AddListener (this, x => {
                
                if (Random.value<0.5f)
                    anima.CrossFade ("stumble_hit_to_head", 0.05f);
                else
                    anima.CrossFade ("stumble_fall_flat", 0.05f);
                Stop ();
            });

            Eve.OnPlayerKilled.AddListener (this, x => Stop ());
        }

        void ChangeState (bool isMove)
        {
            if (isMove == true)
            {
                anima.CrossFade ("Run", 0.1f);
            }
            else
            {
                anima.CrossFade ("Idle", 0.1f);
            }
        }

        void Update ()
        {
            //  не зависит от состояния игры. вынести вьюха
            {
                // Solver height
                ppos.y = roadMgr == null ? 0 : di.GetUpOffset (ppos.z);
                transform.position = ppos;
            }

            if (G.isPause)
                return;

            if (G.deltaTime > 0.1f)
                return;

            if (isFreez)
                return;

            SolveX (Time.smoothDeltaTime);
            SolveY (Time.smoothDeltaTime);
        }

        public void SetPos (Vector3 newPos)
        {
            ppos = newPos;
        }

        public void Stop ()
        {
            isFreez = true;
        }

        public void Run ()
        {
            isFreez = false;
            ChangeState (!isFreez);
        }
    }

    /// <summary>
    /// Движение по Вертикали 
    /// </summary>
    public partial class PlayerMover
    {
        float speedMod_FinishMul01 = 1;
        float mSpeedBase01;

        private void StopY ()
        {
            levelSpeed = 0;
            mSpeedBase01 = 0;
        }

        public void SetStoppedDist (float stoppedDist)
        {
            this.stoppedPos = ppos.z + stoppedDist;

            MTask.Run (this, 1, 0.25f, t => {
                speedMod_FinishMul01 = 1 - t;
            }, () => {
                speedMod_FinishMul01 = 0;
                Stop ();

                Eve.OnPlayerStopped2Fight.FireEvent (null);
            });
        }


        private void SolveY (float dt)
        {
            if (isFreez)
                return;

            // набор скорости вначале
            mSpeedBase01 += ConfDB.bot.BaseAccel * dt;
            mSpeedBase01 = Mathf.Clamp01 (mSpeedBase01);

            float res = botInfo.BaseSpeed * mSpeedBase01 + ConfDB.bot.LongTapVSpeedAdd;// * longSpeedCurved01;
            levelSpeed += botInfo.ShiftTension * (res - levelSpeed) * dt;

            // торможение
            levelSpeed *= speedMod_FinishMul01;

            ppos.z += levelSpeed * dt;
        }
    }

    /// <summary>
    /// Движение по Горизонтали
    /// </summary>
    public partial class PlayerMover
    {
        float shiftDir;
        public float mul;
        private void SolveX (float dt)
        {
            ppos.x += mul * shiftDir * dt;
            ppos.x = Mathf.Clamp (ppos.x, di.CurrStage.LeftBorder, di.CurrStage.RightBorder);
        }

        public void OnInput (GAME_BUTTON btn, bool isPressed, float vel)
        {
            if (btn == GAME_BUTTON.MAIN)
            {
                shiftDir = Mathf.Lerp(shiftDir, vel, 0.5f);
            }
        }
    }
}