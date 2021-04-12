using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Facebook.Unity;
using System.Threading.Tasks;
using System.Threading;


public class SysCore : MonoBehaviour
{
    /// <summary>
    /// Нужно , чтоб можно было любую сцену запустить (меню/игра/редактор) в Editore
    /// </summary>
    static public void SetupFrameRateGlobal()
    {

#if (UNITY_EDITOR) 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

#elif (UNITY_ANDROID) || (UNITY_IPHONE)
        //https://support.unity3d.com/hc/en-us/articles/360000283043-Target-frame-rate-or-V-Blank-not-set-properly
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        GSV.DTIME = 1.2f * 1 / Application.targetFrameRate;//0.048f;
#endif
    }

    static bool isInited = false;
    public static bool needLogin = false;

    // Один на всю игру.
    private void Awake()
    {
        SysCore.SetupFrameRateGlobal();

        if (isInited)
        {
            /*
            Debug.Log("NetCore> ReEnter with Login");

            PlayerPrefs.DeleteAll();
            Debug.Log("NetCore> Clear Cache");

            if (needLogin)
            {
#if (UNITY_ANDROID)
                Debug.Log("NetCore>Awake> ReEnter run> try login");
                GglAccount.Init(() => GglStat.Init(Init));
#else
                Init();
#endif
            }
            else
            {
                Debug.Log("NetCore>Awake> ReEnter run> w/o login");
            }
            */
            DestroyImmediate(gameObject);
            return;
        }

    }

    private void Start()
    {
#if (TRUE)
        FB.Init ();
#else
        Debug.LogWarning ("FB.Init(); ОТКЛЮЧЕНО");
#endif

#if (IRON_PAGE) || (IRON_REWARD) || (IRON_BANNER) 
        /*
        Debug.Log ("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration ();
        */
        // Debug.Log ("unity-script: unity version" + IronSource.unityVersion ());
        // SDK init
        //Debug.Log ("unity-script: IronSource.Agent.init");
        //IronSource.Agent.init ("db616649");
        
        //IronSource.Agent.shouldTrackNetworkState (true);
        IronSource.Agent.init ("db616649", IronSourceAdUnits.BANNER, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
        
        //IntegrationHelper.validateIntegration (activity);
#endif




        isInited = true;
        DontDestroyOnLoad(this.gameObject);


#if (USE_AMPLITUDE)
        Amplitude amp = Amplitude.Instance;
        amp.logging = true;
        amp.init(GSV.AMP_ID);
#endif

        AMPLITUDE.sync_start();


#if UNITY_WEBGL
        Application.ExternalCall("SyncFiles");
#endif

#if (UNITY_ANDROID)
        //  SYNC
        if (PlayerPrefs.HasKey("WAS_LOGINED"))
        {
            Debug.Log("NetCore>Awake> first run> try login");
            GglAccount.Init(() => GglStat.Init(this.Init));
        }
        else
        {
            Debug.Log("NetCore>Awake> first run> w/o login");
            GglStat.Init(this.Init);
        }
#else
        Init();
#endif
    }


    /// <summary>
    /// НА старте нужно чекать юзера (check_user), если нет,
    /// то не сбрасывая статистику JSON_DATA_BASE, зарегаться ещё раз. Хочу базу молча сносить.
    /// + Перезаход под другим пользователем гугла не проработан нормально!!! 
    /// </summary>
    void Init()
    {
        Debug.Log("NetCore> Start server data sync...");

        //  качаем последний кэш
        try
        {
            FilesTool.DownloadNewCache(GSV.APP_VER.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError("DownloadNewCache>" + e.ToString());
        }

        // DEBUG. Эмуляция очистки кэша
        // PlayerPrefs.DeleteAll();

        //  первый запуск или пустой кэш
        if (!PlayerPrefs.HasKey("APP_INSTALLED"))
        {
            //  качаем кэш из APK/IPA, т.е. если вдруг сети не будет, то можно играть на старье.
            try
            {
                Debug.Log("NetCore> First run");
                PlayerPrefs.SetInt("APP_INSTALLED", 1);

                FilesTool.LoadFiles2Cache();

                // Создать новую запись статистики
                ConfDB.stat = new UserStatInfo();

                ConfDB.stat.app_version = GSV.APP_VER;
                ConfDB.stat.player_name = GSV.GOOGLE_DISPLAY_NAME;

                //  сохранить save в кэш
                ConfDB.SaveStat(false);
            }
            catch (System.Exception e)
            {
                Debug.LogError("FirstRun>" + e.ToString());
            }
        }


#if (UNITY_ANDROID)
        if (GSV.GOOGLE_ID != "none" && GSV.GOOGLE_ID != "0")
        {
            PlayerPrefs.SetInt("WAS_LOGINED", 1);

            NetCmd.RegistrationCheck();
            NetCmd.DownloadSave();
            NetCmd.DownloadUserMaps();
        }
#endif

        // Загрузка всех конфигов [ver, item_base, game_props, robike_bot, user_stat]
        //  Там же LoadSave
        // Заглушки "Результаты прохождения" там-же
        try
        {
            ConfDB.LoadConfigsAndSave();
        }
        catch (System.Exception e)
        {
            Debug.LogError("ConfDB.LoadConfigsAndSave>" + e.ToString());
        }

        //  STATS
        {
            ConfDB.stat.cfg_version = 0;
            ConfDB.stat.run_app++;
            ConfDB.SaveStat();

            GglStat.EvLogin();
        }

        
        AMPLITUDE.game_start();


        //  Можно открывать сцену игры
        //  Понеслась !
        SceneLoader.Menu();
    }
}
