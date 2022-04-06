using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame.Factory
{
    public class BossFactory : MonoBehaviour
    {
        [SerializeField] Transform ringBoss;
        
        /*
        [SerializeField] Transform wdg_boss_health_bar;
        public void CreateBossHPWidget ()
        {
            var wdg = Instantiate (wdg_boss_health_bar);
        }
        */

        public IBoss CreateBoss()
        {
            var bossModel = Instantiate (ringBoss).GetComponent<BossRingModel> ();
            return bossModel;
        }
    }
}