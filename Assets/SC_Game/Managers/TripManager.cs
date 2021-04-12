using UnityEngine;
using DG.Tweening;

namespace RBGame
{
    /// <summary>
    /// Система "Заезда". 
    /// </summary>
    public class TripManager : GModel
    {
        public bool IsFinished { get; private set; }
        public float FinishDist => stage.finishDist;

        Sequence msgSequence;
        bool isBotImpacted;

        Vector3 playerPos => bot.position;
        
        private void Start()
        {
            if (!G.isEditMode) // editor
            {
                StageManager.FindStageStat(StageManager.stageID).run_count++;

                //  ЛАЖА. Сгенерить __EVENT
                AMPLITUDE.level_start(StageManager.stageNUM);
            }

            //  Регистрация сообщений(tutor) на карте
            if (stage.msgList.Count != 0)
            {
                msgSequence = DOTween.Sequence();
                msgSequence.
                AppendInterval(1).
                SetLoops(-1).onStepComplete = () =>
                {
                    foreach (var e in stage.msgList)
                    {
                        if (bot.position.z > e.distance)
                        {
                            e.distance = 0xfffffff;
                            FindObjectOfType<GameHUD>().DisplayMsg(Locale.Get(e.text), e.ttl);
                            break;
                        }
                    }
                };
            }
        }

        private void OnDestroy()
        {
            msgSequence?.Kill();
        }

        private void Update ()
        {
            stage.SetViewPoint(playerPos);

            // Проверка на финиш
            if (playerPos.z > FinishDist)
            {
                // Сужаю путь. Чтоб ехал по форватеру
                // ПЛОХО, что здесь по сути 2 функции !SRP, однако
                stage.LeftBorder = Mathf.Clamp(stage.LeftBorder + 3f * Time.deltaTime, -10, -0.1f);
                stage.RightBorder = Mathf.Clamp(stage.RightBorder - 3f * Time.deltaTime, 0.1f, 10);

                if (!IsFinished)
                {
                    IsFinished = true;
                    
                    // Дотянул :(
                    eve.OnPlayerFinished.FireEvent(new PlayerFinished());
                    
                    // Надо вынести в отдельный МАНАГЕР
                    if (coin.totalCount != 0)
                        AMPLITUDE.level_finished(StageManager.stageNUM, (100 * coin.CollectedCount) / coin.totalCount);
                }
            }

            if (!G.isPause)
            {
                // Проверка импакта в SysImpact наверное должна быть
                // Или в качестве стратегии передевать в сам STAGE
                var data = new UpdateData() { playerPos = new Vector2(playerPos.x, playerPos.z), playerRad = 0.3f };
                foreach (var e in stage.instancedItemList)
                {
                    if (e.go != null) // это те кого забрали. Хотя, конечно, нужно перестроить список
                    {
                        e.go.XUpdate(G.deltaTime, data);
                        
                        if (G.isRoundStarted && !isBotImpacted)
                        {
                            //if (e.go.XCollision(data))
                            if (e.go.XCollision (data) && e.go.itemType == MAP_ITEM_TYPE.COIN)
                            {
                                CollisionItemTest(e.go);
                            }
                        }
                    }
                }
            }
        }


        private void CollisionItemTest (IMapItem item)
        {
            eve.OnPlayerInteractItem.FireEvent(new PlayerInteractItem { item = item });

            switch (item.itemType)
            {
                // Монетка
                case MAP_ITEM_TYPE.COIN:
                    coin.Change(+1);
                    SoundManager.get_coin.Play();
                    break;

                // Патроны
                case MAP_ITEM_TYPE.BULLET:
                    VibratorWrapper.Vibrate(100);
                    SoundManager.get_bullet.Play();
                    ///boltManager.Change((int)ConfDB.game.BulletSmallBoxCount);
                    break;

                // Алмаз
                case MAP_ITEM_TYPE.JEWEL:
                    break;

                //  Долбанулись
                case MAP_ITEM_TYPE.BLOCK:
                    eve.OnPlayerImpact.FireEvent(new PlayerImpact());
                    SoundManager.impact.Play(); // Пора манагеру подписываться уже! Не маленький
                    Impact();
                    break;
            }

            // Уничтожить объект
            stage.DeleteItemOnPos(item.pos, true);
        }


        private void Impact()
        {
            CameraJitter.JitIt(1.5f); // потрясём камеру
            isBotImpacted = true;
            AMPLITUDE.level_end(StageManager.stageNUM, false);
        }
    }
}