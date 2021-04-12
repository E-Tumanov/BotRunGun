using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RBGame
{
    public class FireColorBall : GModel
    {
        [SerializeField] ParticleSystem fxTrail;
        [SerializeField] Renderer sphere;

        public int ColorNum { get; set; }
        public void SetColorNum (int colorNum)
        {
            ColorNum = colorNum;
            var clr = di.GetColorByNum (ColorNum);
            fxTrail.startColor = clr;
            sphere.material.color = clr;
        }

        float speed = 50;
        private void Start ()
        {
            transform.position = di.PlayerModel.position + Vector3.up * 1.5f;
            transform.LookAt (di.BattleManager.Boss.GetPivot () + Random.onUnitSphere * 0.5f);
            MTask.Run (this, 0, 2, t => {
                transform.position = transform.position + transform.forward * speed * Time.deltaTime;
            },() => Destroy (this.gameObject));
        }

        private void OnTriggerEnter (Collider other)
        {
            speed = 0;
            GetComponent<SphereCollider> ().enabled = false;
            sphere.gameObject.SetActive (false);
            Destroy (gameObject, 0.1f);
        }
    }
}