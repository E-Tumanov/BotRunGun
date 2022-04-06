using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public class Wdg_CoinCount : GModel
    {
        [SerializeField] TMP_Text coinCount;
        int total;

        private void Awake ()
        {
            //di.CoinManager.onReward += OnChangeCoin;
            gameObject.SetActive (false);
        }

        public void OnChangeCoin(int val)
        {
            total += val;
            coinCount.text = total.ToString ();

            SoundManager.get_coin_sum.Play ();
        }
    }
}