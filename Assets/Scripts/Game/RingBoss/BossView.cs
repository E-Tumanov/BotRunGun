using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace RBGame
{
    /// <summary>
    /// ���. ����
    /// </summary>
    public interface IBossView
    {
        void ActivateGunFire (float delaySec);
        void DisactivateGunFire ();
        void PlayerShot (bool isCrit);
        void Heal ();
        void BossShot ();
        void Exploid ();
        void Victory ();
        void SetColor (int colorNum);
        Vector3 GetPivot ();
        event System.Action<Collider> OnHit;
    }

    public class BossView : GModel, IBossView
    {
        [SerializeField] Transform gunKeeper;
        [SerializeField] Transform stvol;
        [SerializeField] Transform fireHole;
        [SerializeField] Transform face;
        [SerializeField] Transform body;
        [SerializeField] Transform bodyShake;
        [SerializeField] AnimationCurve cv;
        [SerializeField] TextMeshPro textHP;
        [SerializeField] Transform fxBlood;
        [SerializeField] Transform fxBloodNOAH;
        [SerializeField] Transform fxCrush;
        [SerializeField] Transform fxFireBolt;
        [SerializeField] Transform fxFireBoltUlta;
        [SerializeField] Transform fxHealCross;
        [SerializeField] Renderer bodyRender;

        public event System.Action<Collider> OnHit;
        Sequence prepareFireSeq;

        public void SetColor (int colorNum)
        {
            ///bodyRender.material.color = colorStack[colorNum];
        }


        public void DisactivateGunFire ()
        {
            if (prepareFireSeq != null)
            {
                prepareFireSeq.Kill ();
                prepareFireSeq = null;
            }
            SoundManager.alarm_boss_prepare.Stop ();
            gunKeeper.gameObject.SetActive (false);
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        public void ActivateGunFire (float delaySec)
        {
            SoundManager.alarm_boss_prepare.Play ();

            //  ���� ��� ���, �� ������
            if (prepareFireSeq != null)
            {
                prepareFireSeq.Kill ();
                prepareFireSeq = null;
                gunKeeper.gameObject.SetActive (false);
            }

            prepareFireSeq = DOTween.Sequence ()
            .AppendInterval (delaySec)
            .AppendCallback (() => {
                gunKeeper.gameObject.SetActive (true);
            })
            .AppendInterval (4)
            .AppendCallback (() => {
                /*
                //if (SysBullet.count == 0 && 
                if (gunKeeper.gameObject.activeSelf)
                {
                    //GunReady2Fire = true;
                }*/
                // ������� �� �������
            })
            .SetLink (gameObject);

        }


        /// <summary>
        /// ������ �������� ������. 
        /// �.�. ����� ����� ���� ������ ��� � ���� ��������
        /// </summary>
        public void PlayerShot (bool isCrit)
        {
            bodyShake.DOComplete ();
            bodyShake.DOShakePosition (0.5f, 0.25f);

            //bodyShake.DOLocalMove(Vector3.zero, 0.5f);
            //bodyShake.localPosition = bodyShake.localPosition * 0.5f;
            //leftGlaz;
            //rightGlaz;

            var fx = Instantiate (fxBlood, body);
            fx.localPosition = 0.5f * Vector3.back;// + 1 * Vector3.up;
                                                   //+ Vector3.up * (0.25f + 0.5f - Random.value)
                                                   //+ Vector3.right * (0.5f - Random.value);
            /*
            var spos = playerPos + Vector3.up * 0.5f;
            
            if (isCrit)
                Instantiate(fxFireBoltUlta).GetComponent<FireBolt>().Init(spos, fx.position);
            else
                Instantiate(fxFireBolt).GetComponent<FireBolt>().Init(spos, fx.position);
            */

        }


        /// <summary>
        /// ���� ���������
        /// </summary>
        public void Exploid ()
        {
            Instantiate (fxCrush, body.position, Quaternion.identity);
            gameObject.SetActive (false);
        }


        /// <summary>
        /// ������� � ������ (����������� %)
        /// </summary>
        public void BossShot ()
        {
            SoundManager.shoot.volume = 1.1f;
            SoundManager.shoot.Play ();

            // TODO ������ ����(���������) ������� �� �������� ����. ���� ���� �����, ��� ������ ����
            // TODO ������������� ���� (����� ����� 0,2���), ����� ���������!
            Vector3 target = cx.PlayerModel.position;
            var tpos = target + Vector3.forward * 0.2f * ConfDB.bot.BaseSpeed;
            tpos += Vector3.up * 0.6f;// + 0.25f * Random.onUnitSphere;
            Instantiate (fxFireBolt).GetComponent<FireBolt> ().Init (fireHole.position, tpos);
            Instantiate (fxBloodNOAH, tpos, Quaternion.identity);

            // ������
            MTask.Run (this, 0, 0.2f, t => {
                stvol.transform.localPosition = Vector3.forward * cv.Evaluate (t);
            });
        }


        /// <summary>
        /// ������� �����
        /// </summary>
        public void Heal ()
        {
            var fx = GameObject.Instantiate (fxHealCross, transform);
            fx.localPosition = new Vector3 (0.5f * (Random.value * 2 - 1), 1, 0);
        }


        /// <summary>
        /// ���� ������� ������
        /// </summary>
        public void Victory ()
        {
            textHP.text = ":)";
            textHP.gameObject.SetActive (true);
            face.gameObject.SetActive (false);
        }

        public Vector3 GetPivot ()
        {
            return transform.position + Vector3.up * 3.0f;
        }

        void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<FireColorBall> () == null)
            {
                return;
            }
            OnHit?.Invoke (other);
        }

        void Update ()
        {
            float ypos = 0.25f * Mathf.Sin (G.time * 3) + cx.GetUpOffset (0);
            transform.localPosition = new Vector3 (0, ypos, 0);
        }
    }
}

