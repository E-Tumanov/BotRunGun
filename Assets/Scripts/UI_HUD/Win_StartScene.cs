using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UIAnimatorCore;
using System.Configuration;
using UnityEditor;
using TMPro;

/// <summary>
/// Главное меню. Стартовая сцена
/// </summary>
namespace RBGame
{
    // VIEW
    public class Win_StartScene : MonoBehaviour, IWindow
    {
        [SerializeField] Button levelSelect;
        [SerializeField] Button editorSelect;
        [SerializeField] Button rateUs;

        [SerializeField] Text guid_ver_info;
        [SerializeField] Text helloMSG;
        [SerializeField] TMP_Text currLevel;

        [SerializeField] Button tap2Play;

        [SerializeField] Button __logout;
        [SerializeField] Button __login;

        [SerializeField] Button btnPropsMenu;

        [SerializeField] Button rewardVideoButton;

        static bool viewHelloMsg = false;
        public static IEnumerator RateUS() { return null; }


        private void Awake()
        {
            if (ConfDB.stat == null)
                ConfDB.LoadConfigsAndSave();

            G.isRoundStarted = false;

            if (ConfDB.stat.use_sound == 1)
                AudioListener.volume = 1;
            else
                AudioListener.volume = 0;
        }

        void Start()
        {
            btnPropsMenu.onClick.AddListener(() => 
            {
                UIManager.OpenWin(WINDOW.PROPS_MENU, null);
            });

            // просто посмотреть рекламу
            rewardVideoButton.onClick.AddListener(() =>
            {
                ADS.OpenReward(()=>Debug.Log("Reward. OK"), () => Debug.Log("Reward. Cancel"));
            });

            // выход из аккаунта
            __logout.onClick.AddListener(() => 
            {
                // NetCmd.needLogin = true; FIXIT
                GglAccount.LogOut();
                SceneLoader.Intro();
            });
            
            // вход в аккаунт
            __login.onClick.AddListener(() => {
                /*
                GglAccount.LogIn(()=> 
                {
                    //NetCore.inst.DownloadAllFiles();
                });*/
            });

            /*
            var bv = PlayerSettings.bundleVersion;
            guid_ver_info.text = $"bv:{bv} v: {GSV.APP_VER}[{ConfDB.ver.config_version}] ID: {GSV.USER_ID}";
            */
            guid_ver_info.text = $"v: {GSV.APP_VER}[{ConfDB.ver.config_version}] ID: {GSV.USER_ID}";

#if !(ZX_RELEASE) && (UNITY_ANDROID)
            guid_ver_info.text += " G: " + GS.GOOGLE_ID;
#endif

            // Сообщение только один раз при старте приложения
            if (!viewHelloMsg)
            {
                helloMSG.text = ConfDB.ver.msg;
                viewHelloMsg = true;
            }

            //  Выбор уровня
            levelSelect.onClick.AddListener(() =>
            {
                UIManager.OpenWin(WINDOW.STAGE_SELECT_GRID, null);
            });

            //  Запуск редактора
            editorSelect.onClick.AddListener(() =>
            {
                ConfDB.IncEnterEditor(); // TODO: Аналитика. Убрать из DB
                SceneLoader.Editor();
            });

            // RATE US
            rateUs.onClick.AddListener(() =>
            {
                Application.OpenURL("market://details?id=com.tumanoid.allcashgames");
                //StartCoroutine(GglAccount.RateUS()); // ДОДЕЛАТЬ
            });

            tap2Play.onClick.AddListener(() =>
            {
                SceneLoader.Game();
            });

            //  Проверка тутора
            G.isTutorEnable = ConfDB.stat.tutor_complete == 0;

            //Debug.Log(G.isTutorEnable?"IS TUTOR" : "NO_TUTOR");

            //  Поиск нужной(игровой) карты
            StageManager.Init(G.isTutorEnable);
            G.stageFileName = StageManager.currStageFileName;
            
            //  Напишем, какой уровень играем
            currLevel.text = $"{Locale.Get("level")} {StageManager.stageNUM.ToString()}";
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }
}