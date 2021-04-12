using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    public class Wdg_StageProgress : GModel
    {
        [SerializeField] Transform fill;
        [SerializeField] Text stageNum;
        

        void Start()
        {
            eve.OnPlayerInteractItem.AddListener(this, x => { });

            //  не отписываюсь. просто скрываю до конца заезда
            eve.OnPlayerFinished.AddListener(this, x =>
            {
                MTask.Run(this, 0.3f, () => gameObject.SetActive(false));
                //gameObject.SetActive(false);
                //DOTween.Sequence().AppendInterval(0.3f).OnComplete(() => gameObject.SetActive(false)).SetLink(gameObject);
            });

            stageNum.text = StageManager.stageNUM.ToString();
            fill.localScale = new Vector3(0, 1, 1);
        }


        void Update()
        {
            float s = Mathf.Clamp01(bot.position.z / (0.1f + trip.FinishDist));
            fill.localScale = new Vector3(s, 1, 1);
        }
    }
}