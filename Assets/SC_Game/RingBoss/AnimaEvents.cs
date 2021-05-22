using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    public class AnimaEvents : GModel
    {
        public event System.Action OnAimed = delegate { };

        [SerializeField] Transform gun;
        public void OnGunPrepared ()
        {
            var srot = Quaternion.Euler (-90, 15, 0) * Vector3.forward;
            var trot = (di.PlayerModel.position - gun.position).normalized;
            MTask.Run (this, 0, 2, t =>
            {
                gun.forward = Vector3.Lerp (srot, trot, t * 1.2f);
            },
            () => {
                OnAimed ();
            });
        }
    }
}