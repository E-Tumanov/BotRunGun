using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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

        // [DEP] это если будет много, то узнать, кто стрелял. Хотя в "команде" это можно прописать
        // public event System.Action<float> onFire; 

        [SerializeField] BossView view;
        [SerializeField] AnimaEvents animaEvents;
        [SerializeField] int totalPlateCounter;
        int currPlateCount;
        [SerializeField] int ringRad = 4;
        [SerializeField] Transform pivotHP;
        [SerializeField] Transform pivotPlate;
        [SerializeField] BossColorPlate bronPrefab;
        [SerializeField] BossColorPlate platePrefab;

        List<BossColorPlate> bronList = new List<BossColorPlate> ();
        List<BossColorPlate> colorPlateList = new List<BossColorPlate> ();

        float pivotHPRotateSpeed;
        float pivotPlateRotateSpeed;
        bool isGunActive;

        private void Start ()
        {
            transform.position = (di.TripManager.FinishDist + 30) * Vector3.forward;

            pivotHPRotateSpeed = 30 + Random.value * 30;
            pivotPlateRotateSpeed = 80 + Random.value * 30;

            CurrHP = PrevHP = MaxHP = di.BallsManager.TotalCount * di.BossInfo.hp;
            eve.OnBossRecieveDamage.FireEvent (new BossRecieveDamage { boss = this });
            view.OnHit += OnRedCubeHit;

            animaEvents.OnAimed += BossRingModel_OnAimed;

            eve.OnPlayerFinished.AddListener (this, x => {
                //MTask.Run (0.5f, () => di.BossFactory.CreateBossHPWidget ());
            });

            eve.OnPlayerKilled.AddListener (this, x =>
            {
                OnPlayerKilled ();
            });

            eve.OnBuyBullet.AddListener (this, x =>
            {
                if (x.isStarted)
                    OnStartBuyBullet ();
                if (x.value > 0)
                    OnBuyBullet ();
                if (x.isCanceled)
                    ActivateGunFireAfter (1.0f);
            });

            eve.OnPlayerShot.AddListener (this, x =>
            {
                if (x.isMissShot)
                    ActivateGunFireAfter (2);
            });


            CreateRing ();
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
            eve.OnBossShot.FireEvent (new BossShot ());// Сообщим, что босс "пальнул"
        }


        //  Создать защитное кольцо
        void CreateRing ()
        {
            if (ringRad < 0)
                ringRad = 0;
            bronList.Clear ();

            foreach (Transform e in pivotHP.transform)
                MTask.Run (this, 0.05f, () => GameObject.DestroyImmediate (e.gameObject));
            foreach (Transform e in pivotPlate.transform)
                MTask.Run (this, 0.05f, () => GameObject.DestroyImmediate (e.gameObject));


            currPlateCount = totalPlateCounter;
            
            foreach (var e in di.CurrStage.bossInfo.brona)
            {
                var go = Instantiate (bronPrefab, pivotPlate, false);
                go.gameObject.SetActive (true);
                go.transform.localRotation = Quaternion.AngleAxis (e.polarR, Vector3.up);
                go.transform.localPosition = go.transform.forward * e.polarH;
                go.Init (-1, 1);
                go.transform.localScale = Vector3.one * e.size;
                go.OnHit += OnPlateHit;
                bronList.Add (go);

                var rotator = go.GetComponent<PlateRotator> ();
                rotator.polarH = e.polarH;
                rotator.polarR = e.polarR;
                rotator.rotSpeed = e.rotSpeed;
            }
        }


        // Попали в сердце
        void OnRedCubeHit (Collider obj)
        {
            //  если игрок убит, то уже ничего не происходит
            if (di.BattleManager.PlayerHP <= 0)
                return;

            PrevHP = CurrHP;
            CurrHP = Mathf.Clamp (CurrHP - 1, 0, MaxHP);


            //progressHPLine

            eve.OnBossRecieveDamage.FireEvent (new BossRecieveDamage { boss = this });

            view.PlayerShot (false);

            if (CurrHP == 0)
            {
                animaEvents.OnAimed -= BossRingModel_OnAimed;

                view.Exploid (); // скрывает gameObject. не удаляет
                eve.OnBossKilled.FireEvent (new BossKilled ());

                foreach (var e in bronList)
                    Destroy (e.gameObject);
                bronList.Clear ();
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


        void OnColorHit (BossColorPlate plate, Collider boltObject)
        {
            var bolt = boltObject.GetComponent<FireColorBall> ();
            if (plate.ColorNum == bolt.ColorNum)
            {
                Destroy (plate.gameObject);
                currPlateCount--;

                if (currPlateCount == 0)
                    eve.OnBossKilled.FireEvent (new BossKilled ());
            }
        }


        private void Update ()
        {
            //transform.position = (di.TripManager.FinishDist + 30) * Vector3.forward;
        }


        /// <summary>
        /// RENAME: Victory
        /// </summary>
        private void OnPlayerKilled ()
        {
            view?.Victory ();
        }


        /// <summary>
        /// RENAME: CancelPrepare2Fire
        /// </summary>
        private void OnStartBuyBullet ()
        {
            view?.DisactivateGunFire ();
        }


        /// <summary>
        /// Обработчик. Игрок закупился патронами
        /// </summary>
        private void OnBuyBullet ()
        {
        }
    }
}