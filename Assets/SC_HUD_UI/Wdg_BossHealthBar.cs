using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace RBGame
{
    /// <summary>
    /// Виджет. Кол-во ХП у босса
    /// </summary>
    public class Wdg_BossHealthBar : GModel
    {
        [SerializeField] Transform fill;
        [SerializeField] Transform fillDelay;
        //[SerializeField] Text hp_text;

        void Awake ()
        {
            //FindObjectOfType<GameHUD> ().Append (transform);

            eve.OnBossRecieveDamage.AddListener (this, x =>
            {
                var currHP = x.boss.CurrHP;
                var prevHP = x.boss.PrevHP;
                var maxHP = x.boss.MaxHP;
                if (currHP == 0)
                {
                    Destroy (gameObject);
                }
                else
                {
                    fill.DOKill ();
                    fillDelay.DOKill ();
                    if (currHP < prevHP)
                    {
                        fill.localScale = new Vector3 (currHP / maxHP, 1, 1);
                        fillDelay.DOScaleX (currHP / maxHP, 0.5f);
                    }
                    else
                    {
                        fillDelay.localScale = new Vector3 (currHP / maxHP, 1, 1);
                        fill.DOScaleX (currHP / maxHP, 0.5f);
                    }

                    //  Всегда округяй в большую сторону.
                    //hp_text.text = Mathf.CeilToInt (currHP).ToString ();
                }
            });
        }
    }
}