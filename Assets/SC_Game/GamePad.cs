using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 
/// Система Ввода. Основные кнопки управления. 
/// Думаю, если система будет посложней, то сильно не должно менятся.
/// Пример. Геймпад (левый/правый), менюшка, громкость, верхний закупочный бар. 
/// Может быть слоты под уникальные скилы
/// 
/// UNITY/ Здесь еще используется "мышиное" программирование. Надобы переделать
/// </summary>
namespace RBGame
{
    public enum GAME_BUTTON
    {
        MAIN, LEFT, RIGHT, MENU, EDITOR, RESTART, SOUND_TOGGLE,
        _end
    }


    public interface IGamepad
    {
        void AddListener(System.Action<GAME_BUTTON, bool> listener);
    }


    public class GamePad : MonoBehaviour, IGamepad, IPointerDownHandler, IPointerUpHandler
    {
        event System.Action<GAME_BUTTON, bool> OnButtonTouch = delegate { };

        [SerializeField] Button menuBtn;
        [SerializeField] Button editorBtn;
        [SerializeField] Button restartBtn;

        [SerializeField] Transform singleButton;

        public void AddListener(System.Action<GAME_BUTTON, bool> listener)
        {
            OnButtonTouch += listener;
        }

        private void Start()
        {
            // Кнопка "РЕДАКТ"
            editorBtn.onClick.AddListener(() => OnButtonTouch(GAME_BUTTON.EDITOR, true));
            editorBtn.gameObject.SetActive(G.isEditMode);

            // Кнопка "РЕСТАРТ"
            restartBtn.onClick.AddListener(() => OnButtonTouch(GAME_BUTTON.RESTART, true));
            restartBtn.gameObject.SetActive(G.isEditMode);
#if (UNITY_EDITOR)
            restartBtn.gameObject.SetActive(false);
#endif

            // Кнопка "МЕНЮ"
            menuBtn.onClick.AddListener(() => OnButtonTouch(GAME_BUTTON.MENU, true));
            menuBtn.gameObject.SetActive(false);
            /*
            // Сброс мяча
            ballResetBtn.onClick.AddListener(() => OnButtonTouch(GAME_BUTTON._end, true));
            ballResetBtn.gameObject.SetActive(false);
            */
            //  Кнопки "УПРАВЛЕНИЕ" - главные
            var isDao = ConfDB.stat.use_dao_control == 1;
            singleButton.gameObject.SetActive(true);

/*
#if !(ZX_RELEASE) // если не в релизе, то видна всегда.
        menuBtn.gameObject.SetActive(true);
        restartBtn.gameObject.SetActive(true);
        ballResetBtn.gameObject.SetActive(true);
#endif*/
        }

        class PInfo
        {
            public PointerEventData pdata;
            public int id;
            public Vector2 pos;
            public float ptime;
        }

        PointerEventData point;

        bool leftBtPressed;
        bool rightBtPressed;
        float lastX;

        public void OnPointerDown(PointerEventData pdata)
        {
            if (point == null)
            {
                point = pdata;
                lastX = point.position.x;
                
                OnButtonTouch(GAME_BUTTON.MAIN, true);
            }
        }

        public void OnPointerUp(PointerEventData pdata)
        {
            if (point != null)
            {
                leftBtPressed = false;
                rightBtPressed = false;
                point = null;

                OnButtonTouch(GAME_BUTTON.MAIN, false);
            }
        }
        
        void SolvePoint()
        {
            OnButtonTouch(GAME_BUTTON.LEFT, leftBtPressed);
            OnButtonTouch(GAME_BUTTON.RIGHT, rightBtPressed);

            if (point == null)
                return;

            if (point.position.x - lastX > 25)
            {
                lastX = point.position.x;
                rightBtPressed = true;
                leftBtPressed = false;
            }
            else if (point.position.x - lastX < -25)
            {
                lastX = point.position.x;
                leftBtPressed = true;
                rightBtPressed = false;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            G.SetPause(pauseStatus);
        }

        void Update()
        {
            SolvePoint();

#if (UNITY_EDITOR)
            OnButtonTouch (GAME_BUTTON.LEFT, Input.GetKey(KeyCode.Q));
            OnButtonTouch (GAME_BUTTON.RIGHT, Input.GetKey (KeyCode.E));
#endif            
            /*
            //  Для SmartTV посмотреть код нажатой клавки
            foreach (KeyCode kcode in KeyCode.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                    coinCount.text = Input.touchCount + "." + kcode.ToString();
            }
            */

            //======================================
            //  SmartTV
            //  Кнопки джойстика/пульта
            // Только DAO CONTROL
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) ||
                    Input.GetKeyDown(KeyCode.Joystick2Button0) ||
                    Input.GetKeyDown(KeyCode.Joystick3Button0)
                    )
                {
                    OnButtonTouch(GAME_BUTTON.MAIN, true);
                }
                if (Input.GetKeyUp(KeyCode.Joystick1Button0) ||
                    Input.GetKeyUp(KeyCode.Joystick2Button0) ||
                    Input.GetKeyUp(KeyCode.Joystick3Button0)
                    )
                {
                    OnButtonTouch(GAME_BUTTON.MAIN, false);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnButtonTouch(GAME_BUTTON.RESTART, true);
            }

            // Пауза по ВНЕШНЕЙ кнопке
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                G.SetPause(!G.isPause);
            }
        }
    }
}