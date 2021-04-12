using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public class Wdg_CoinInfo : MonoBehaviour
    {
        [SerializeField] Text coinCount;

        void Start()
        {
            coinCount.text = ConfDB.stat.total_coins.ToString();
        }
    }
}