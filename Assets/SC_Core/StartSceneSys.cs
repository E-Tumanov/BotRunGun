using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace RBGame
{
    /// <summary>
    /// RENAME  
    /// 
    /// Это  "Проверка на новые версии в сторе"
    /// 
    /// </summary>
    public class StartSceneSys : MonoBehaviour
    {
        [SerializeField] Button downloadApp;


        private void Awake()
        {
            GglStat.EvLogin();

            G.isEditMode = false;
        }


        void Start()
        {
            ConfDB.LoadConfigsAndSave();

            if (GSV.ACCESS_VER <= GSV.APP_VER)
            {
                UIManager.OpenWin(WINDOW.MAIN_MENU, null);
            }
            else
            {
                downloadApp.gameObject.SetActive(true);
                downloadApp.onClick.AddListener(() =>
                {
#if UNITY_IPHONE
                Application.OpenURL("itms-apps://apps.apple.com/app/id1542354035");
#endif

#if UNITY_ANDROID
                Application.OpenURL("market://details?id=com.tumanoid.allcashgames");
#endif
            });
            }
        }
    }
}