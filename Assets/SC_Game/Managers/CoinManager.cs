using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    public class CoinManager : GModel
    {
        //public event System.Action<int, int> changeCount = delegate { };
        public event System.Action<int> onReward = delegate { };

        // Всего на карте монет
        public int totalCount;

        //  Количество собраных монет
        public int CollectedCount { get; private set; }

        public int PrintCount { get; private set; }

        [SerializeField] Wdg_CollectItem wdgCollect;
        [SerializeField] Wdg_CoinCount wdgCounter;

        private void Start ()
        {
            totalCount = di.CurrStage.CountItemByType ("COIN");
            CollectedCount = di.CurrStage.coinPreReward;

            Eve.OnBossKilled.AddListener (this, x => 
            {
                MTask.Run (this, 2, () => 
                {
                    StartCollector ();
                    SaveResult ();
                });
            });
        }


        void StartCollector ()
        {
            wdgCounter.OnChangeCoin (0);
            wdgCounter.gameObject.SetActive (true);

            var t = 15 + CollectedCount / 5;

            //  Начислтить красво
            for (int i = 0; i < t; i++)
            {
                PrintCount += 5;
                MTask.Run (this, Random.value * t / 10.0f, () => wdgCollect.Emmit (() => wdgCounter.OnChangeCoin (5)));
            }
        }


        void SaveResult ()
        {
            //  РЕЗУЛЬТАТ ПО МОНЕТАМ
            if (StageManager.stageID != 0)
            {
                var stageRes = StageManager.FindStageStat (StageManager.stageID);
                stageRes.is_complete += 1;

                //  посчитаем новый рекорд
                bool newRecord = false;
                {
                    if (PrintCount > stageRes.max_coin)
                    {
                        newRecord = true;
                        stageRes.max_coin = PrintCount;
                    }
                }
            }

            // SAVED 2 SERVER
            ConfDB.stat.total_coins += PrintCount;
            ConfDB.SaveStat ();
        }


        public void Change(int value)
        {
            if (value == 0)
                return;

            var prev = CollectedCount;
            CollectedCount = Mathf.Clamp(CollectedCount + value, 0, 10000000);
            //changeCount(CollectedCount, prev);
        }
    }
}
