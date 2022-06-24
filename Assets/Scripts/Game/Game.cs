using DG.Tweening;
//using Facebook.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace RBGame
{

    /*
    
    MetaGame Manger уже нужен
    Сохранения
    Прогресс
    Статистика
    Аналитика возможно
     
     */

    /// <summary>
    ///
    /// </summary>
    public class Game : GModel
    {
        private void Start()
        {
            G.isRoundStarted = true;

            SysCore.SetupFrameRateGlobal();

            Eve.OnBossKilled.AddListener (this, x => {
                Eve.OnGameOver.FireEvent (true);
                OnVictory ();
            });

            Eve.OnPlayerImpact.AddListener (this, x => {
                Eve.OnGameOver.FireEvent (false);
                OnFail ();
            });

            Eve.OnPlayerKilled.AddListener (this, x => {
                Eve.OnGameOver.FireEvent (false);
                OnFail ();
            });

            SoundManager.Init();

            cx.GamePad.OnButtonTouch += GamePad_OnButtonTouch;
        }

        void GamePad_OnButtonTouch (GAME_BUTTON btn, bool isPressed)
        {
            if (isPressed && btn == GAME_BUTTON.EDITOR)
            {
                SceneLoader.Editor ();
            }
        }

        void OnDestroy()
        {
            G.ResetStaticData();

            var c = DOTween.KillAll(false);
            if (false && c > 0)
                Debug.LogWarning("DOTween.KillAll> count: " + c.ToString());
        }

        void OnFail ()
        {
            AMPLITUDE.level_end (StageManager.stageNUM, false);            
        }

        void OnVictory() 
        {
            // fixit. Пускай сами подписываются
            GglStat.EvLevelDone((int)StageManager.stageNUM, true, false);
            AMPLITUDE.level_end(StageManager.stageNUM, true);

            if (G.isTutorEnable && StageManager.stageNUM == 1)
            {
                ConfDB.stat.tutor_complete = 1;
                try
                {
                    var parameters = new Dictionary<string, object>();
                    //parameters[AppEventParameterName.Success] = true;
                    //FB.LogAppEvent(AppEventName.CompletedTutorial, 1, parameters);
                    Debug.Log("FB> Send TutorComplete. OK");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("FB> Send TutorComplete err. " + e.ToString());
                }
            }

            ConfDB.SaveStat ();
        }
    }
}