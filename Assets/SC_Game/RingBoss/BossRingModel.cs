using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    public interface IBoss
    {
        float CurrHP { get; }
        float PrevHP { get; }
        float MaxHP { get; }
        void Damage (int colorNum, float dmg, bool isCrit);
        void Exploid ();
        void ActivateGunFireAfter (float sec);
        Vector3 GetPivot();
    }

    public class BossRingModel : GModel, IBoss
    {
        public float CurrHP { get; private set; }
        public float PrevHP { get; private set; }
        public float MaxHP { get; private set; }

        public void Damage (int colorNum, float dmg, bool isCrit) { }
        public void Exploid () { }

        [SerializeField] BossView view;
        [SerializeField] AnimaEvents animaEvents;

        bool isGunActive;

        private void Start ()
        {
            transform.position = (di.TripManager.FinishDist + 30) * Vector3.forward;

            CurrHP = PrevHP = MaxHP = di.BallsManager.TotalCount * di.BossInfo.hp + 1; // +1 не бывает босcа с 0 HP
            Eve.OnBossRecieveDamage.FireEvent (new BossRecieveDamage { boss = this });
            view.OnHit += OnRedCubeHit;

            animaEvents.OnAimed += BossRingModel_OnAimed;

            Eve.OnPlayerFinished.AddListener (this, x => {
                //MTask.Run (0.5f, () => di.BossFactory.CreateBossHPWidget ());
            });

            Eve.OnPlayerKilled.AddListener (this, x =>
            {
                OnPlayerKilled ();
            });

            Eve.OnBuyBullet.AddListener (this, x =>
            {
                if (x.isStarted)
                    OnStartBuyBullet ();
                if (x.value > 0)
                    OnBuyBullet ();
                if (x.isCanceled)
                    ActivateGunFireAfter (0.5f);
            });

            Eve.OnPlayerShot.AddListener (this, x =>
            {
                if (x.isMissShot)
                    ActivateGunFireAfter (0.5f);
            });


            CreateRing ();
        }

        //  Создать защитное кольцо
        void CreateRing ()
        {
            
            foreach (var e in di.CurrStage.bossInfo.brona)
            {
                var rg = Resources.Load<Transform> (e.mtype);
                if (rg != null)
                {
                    var tt = Instantiate (rg, transform, false);
                    tt.transform.localPosition = Vector3.up * 1.5f;
                }
                else
                {
                    Debug.LogError ($"CreateRing> not found : {e.mtype}");
                }
            }
        }


        public Vector3 GetPivot ()
        {
            return view.GetPivot ();
        }


        // Пушка наведена! Можно стрелять
        private void BossRingModel_OnAimed ()
        {
            if (CurrHP <= 0)
                return;
            
            view.BossShot ();
            Eve.OnBossShot.FireEvent (null);// Сообщим, что босс "пальнул"
        }


        // Попали в сердце
        void OnRedCubeHit (Collider obj)
        {
            //  если игрок убит, то уже ничего не происходит
            if (di.BattleManager.PlayerHP <= 0)
                return;

            PrevHP = CurrHP;
            CurrHP = Mathf.Clamp (CurrHP - 1, 0, MaxHP);

            Eve.OnBossRecieveDamage.FireEvent (new BossRecieveDamage { boss = this });

            view.PlayerShot (false);

            if (CurrHP == 0)
            {
                animaEvents.OnAimed -= BossRingModel_OnAimed;

                view.Exploid (); // скрывает gameObject. не удаляет
                Eve.OnBossKilled.FireEvent (null);
            }
        }


        public void ActivateGunFireAfter (float sec)
        {
            if (isGunActive)
                return;

            view.ActivateGunFire (sec);
            isGunActive = true;

            // TODO: если закупился
            //view.DisactivateGunFire();
        }

        
        void OnPlateHit (BossColorPlate plate, Collider boltObject)
        {
            //var bolt = boltObject.GetComponent<FireColorBall> ();
            ActivateGunFireAfter (0.05f);
        }

        // Victory
        private void OnPlayerKilled ()
        {
            view?.Victory ();
        }


        // CancelPrepare2Fire
        private void OnStartBuyBullet ()
        {
            view?.DisactivateGunFire ();
        }


        // Обработчик. Игрок закупился патронами
        private void OnBuyBullet ()
        {
        }
    }
}