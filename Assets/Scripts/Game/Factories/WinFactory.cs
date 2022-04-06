using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RBGame
{
    public interface IWinPresenter
    {
        void Init(JSONNode data);
    }

    public interface IWindow
    {
        void DestroyWindow();
    }

    public enum WINDOW
    {
        MAIN_MENU = 1,
        STAGE_SELECT_GRID = 2,
        GAME_PAUSE_IMPACT = 4,
        BUY_BULLET = 5,
        GAME_DEFEAT = 6,
        GAME_VICTORY = 7,
        PROPS_MENU = 8,
        __end = 255
    }

    /// <summary>
    /// Manager
    /// </summary>
    public static class UIManager
    {
        static Factory.WinFactory winFactory;
        public static void OpenWin(WINDOW win, JSONNode data)
        {
            winFactory = GameObject.FindObjectOfType<Factory.WinFactory>();
            winFactory?.Open(win, data);
        }
    }
}



namespace RBGame.Factory
{
    /// <summary>
    /// Создать/Закрыть окно
    /// </summary>
    public class WinFactory : MonoBehaviour
    {
        [System.Serializable]
        public class WindowDescription
        {
            public WINDOW type;
            public GameObject prefab;
        }

        // Префабы окон
        [SerializeField] List<WindowDescription> windows;

        IWindow currWin;
        IWinPresenter currPresenter;
        Transform currCanvas;

        void Awake()
        {
            currCanvas = FindObjectOfType<Canvas>().transform; // найдём canvas
            Assert.IsNotNull(currCanvas); // Должен быть хотябы один.
        }

        public void Open(WINDOW win, JSONNode data)
        {
            Assert.IsNotNull(currCanvas);

            if (currWin != null)
            {
                currWin.DestroyWindow();
            }

            //  clear
            currWin = null;
            currPresenter = null;

            // create
            currWin = Create(win);
            currPresenter?.Init(data);
        }

        private IWindow Create(WINDOW win)
        {
            var windowInfo = windows.Find(rez => rez.type.Equals(win));
            if (windowInfo != null)
            {
                return Instantiate(windowInfo.prefab, currCanvas, false).GetComponent<IWindow>();
            }

            return null;
        }
    }
}