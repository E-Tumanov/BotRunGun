using UnityEngine;
using DG.Tweening;

namespace RBGame
{
    /// <summary>
    /// ������� "������". 
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

                //  ����. ��������� __EVENT
                AMPLITUDE.level_start(StageManager.stageNUM);
            }

            //  ����������� ���������(tutor) �� �����
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

            bot.Run ();
        }

        private void OnDestroy()
        {
            msgSequence?.Kill();
        }

        private void Update ()
        {
            stage.SetViewPoint(playerPos);

            // �������� �� �����
            if (playerPos.z > FinishDist)
            {
                // ����� ����. ���� ���� �� ���������
                // �����, ��� ����� �� ���� 2 ������� !SRP, ������
                stage.LeftBorder = Mathf.Clamp(stage.LeftBorder + 3f * Time.deltaTime, -10, -0.1f);
                stage.RightBorder = Mathf.Clamp(stage.RightBorder - 3f * Time.deltaTime, 0.1f, 10);

                if (!IsFinished)
                {
                    IsFinished = true;

                    // ������� �� ������
                    Eve.OnPlayerFinished.FireEvent(null);
                    
                    // ���� ������� � ��������� �������
                    if (coin.totalCount != 0)
                        AMPLITUDE.level_finished(StageManager.stageNUM, (100 * coin.CollectedCount) / coin.totalCount);
                }
            }

            if (!G.isPause)
            {
                // �������� ������� � SysImpact �������� ������ ����
                // ��� � �������� ��������� ���������� � ��� STAGE

                // ����� ������ �� ������ ������� ���������
                var data = new UpdateData() { playerPos = new Vector2(playerPos.x, playerPos.z), playerRad = 0.2f };
                foreach (var e in stage.instancedItemList)
                {
                    if (e.go != null) // ��� �� ���� �������. ����, �������, ����� ����������� ������
                    {
                        e.go.XUpdate(G.deltaTime, data);
                        
                        if (G.isRoundStarted)
                        {
                            if (e.go.XCollision(data))
                            {
                                CollisionItemTest(e.go);
                            }
                        }
                    }
                }
            }
        }


        public void CollisionItemTest (IMapItem item)
        {
            if (isBotImpacted)
                return;

            Eve.OnPlayerInteractItem.FireEvent(new PlayerInteractItem { item = item });

            switch (item.itemType)
            {
                // �������
                case MAP_ITEM_TYPE.COIN:
                    coin.Change(+1);
                    SoundManager.get_coin.Play();
                    // ���������� ������
                    stage.DeleteItemOnPos(item.pos, true);
                    break;

                // �������
                case MAP_ITEM_TYPE.BULLET:
                    VibratorWrapper.Vibrate(100);
                    SoundManager.get_bullet.Play();
                    ///boltManager.Change((int)ConfDB.game.BulletSmallBoxCount);
                    break;

                // �����
                case MAP_ITEM_TYPE.JEWEL:
                    // ���������� ������
                    stage.DeleteItemOnPos (item.pos, true);
                    break;

                //  �����������
                case MAP_ITEM_TYPE.BLOCK:
                    Eve.OnPlayerImpact.FireEvent(null);
                    SoundManager.impact.Play(); // ���� �������� ������������� ���! �� ���������
                    Impact();
                    break;
            }
        }


        private void Impact()
        {
            isBotImpacted = true;
        }
    }
}