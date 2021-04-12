using DG.Tweening;
using Facebook.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace RBGame
{
    /// <summary>
    ///
    /// </summary>
    public class Game : GModel
    {
        private void LateUpdate()
        {
            G.SolveTime();
            GSV.DTIME = 1.2f * Time.smoothDeltaTime;
        }

        private void Start()
        {
            SysCore.SetupFrameRateGlobal();

            G.isRoundStarted = true;

            // линкуем кнопки с игроком
            gamepad.AddListener(GamePad); 

            // стата при победе
            eve.OnBossKilled.AddListener(this, _ => { OnVictory(); });

            // ПЕРЕДЕЛАТЬ , там старьё!!!!!!!!!!!!!!!
            /// Бл, Надо срочно переделать. Нахер это здесь вообще. Да и сам INIT это с 90х
            SoundManager.Init();
        }

        private void OnDestroy()
        {
            G.ResetStaticData();

            var c = DOTween.KillAll(false);
            if (false && c > 0)
                Debug.LogWarning("DOTween.KillAll> count: " + c.ToString());
        }

        /// <summary>
        /// Босс уничтожен/ Конец раунда
        /// </summary>
        private void OnVictory() 
        {
            bot.SetStoppedDist(ConfDB.game.StoppedDist);

            // ПЕРЕДЕЛАТЬ. Пускай сами подписываются
            GglStat.EvLevelDone((int)StageManager.stageNUM, true, false);
            AMPLITUDE.level_end(StageManager.stageNUM, true);
            

            if (G.isTutorEnable && StageManager.stageNUM == 1)
            {
                ConfDB.stat.tutor_complete = 1;
                try
                {
                    var parameters = new Dictionary<string, object>();
                    //parameters[AppEventParameterName..Content] = contentData;
                    //parameters[AppEventParameterName.ContentID] = contentId;
                    parameters[AppEventParameterName.Success] = true;
                    FB.LogAppEvent(AppEventName.CompletedTutorial, 1, parameters);
                    Debug.Log("FB> Send TutorComplete. OK");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("FB> Send TutorComplete err. " + e.ToString());
                }
            }
            ConfDB.SaveStat ();
        }


        /// <summary>
        /// Линкуем ядро с GAMEPAD
        /// </summary>
        private void GamePad(GAME_BUTTON btn, bool isPressed)
        {
            switch (btn)
            {
                case GAME_BUTTON.EDITOR: SceneLoader.Editor(); break;
                case GAME_BUTTON.RESTART: SceneLoader.Game(); break;
                case GAME_BUTTON.MENU: SceneLoader.Menu(); break;
                default: eve.OnButtonPress.FireEvent(new ButtonPress { btn = btn, isPressed = isPressed }); break;
            }
        }
    }
}