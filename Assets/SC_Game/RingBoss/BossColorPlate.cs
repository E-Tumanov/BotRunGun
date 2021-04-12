using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    public class BossColorPlate : GModel
    {
        public event System.Action<BossColorPlate, Collider> OnHit = delegate { };
        public int ColorNum { get; set; }

        [SerializeField] Transform fxImpactBrona;
        

        public void Init (int colorNum, int hp)
        {
            // Это чОрные сферы!
            /*
            if (colorNum >= 0)
            {
                this.ColorNum = colorNum;
                GetComponent<Renderer> ().material.color = di.GetColorByNum (colorNum);
            }
            else
            {
                //GetComponent<Renderer> ().material.color = new Color (0.25f, 0.25f, 0.3f);
            }*/
        }

        float nextTime;

        private void OnTriggerEnter (Collider other)
        {
            //  чтоб не спамить fx
            if (nextTime < Time.time)
            {
                nextTime = Time.time + 2;
                Instantiate (fxImpactBrona, transform);
            }
            OnHit (this, other);
        }
    }
}