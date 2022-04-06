using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    /// <summary>
    /// До конца уровня. ниточка
    /// </summary>
    public class Wdg_StageProgress : GModel
    {
        [SerializeField] Transform fill;
        [SerializeField] Text stageNum;

        void Start()
        {
            //  не отписываюсь. просто скрываю до конца заезда
            Eve.OnPlayerFinished.AddListener(this, x =>
            {
                MTask.Run(this, 0.3f, () => gameObject.SetActive(false));
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