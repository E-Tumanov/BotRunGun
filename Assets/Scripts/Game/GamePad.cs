using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
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
        event System.Action<GAME_BUTTON, bool> OnButtonTouch;
        event System.Action<GAME_BUTTON, bool, float> OnButtonTouchVel;
    }


    public class GamePad : MonoBehaviour, IGamepad, IPointerDownHandler, IPointerUpHandler
    {
        public event System.Action<GAME_BUTTON, bool> OnButtonTouch = delegate { };
        public event System.Action<GAME_BUTTON, bool, float> OnButtonTouchVel = delegate { };
        // <btn, start_pos, cur_pos>
        public event System.Action<GAME_BUTTON, float, float> OnDragX = delegate { };

        [SerializeField] Button menuBtn;
        [SerializeField] Button editorBtn;
        [SerializeField] Button restartBtn;

        [SerializeField] Transform singleButton;

        private void Start ()
        {
            // Кнопка "РЕДАКТ"
            editorBtn.onClick.AddListener (() => OnButtonTouch (GAME_BUTTON.EDITOR, true));
            editorBtn.gameObject.SetActive (G.isEditMode);

            // Кнопка "РЕСТАРТ"
            /*
            restartBtn.onClick.AddListener (() => OnButtonTouch (GAME_BUTTON.RESTART, true));
            restartBtn.gameObject.SetActive (G.isEditMode);
#if (UNITY_EDITOR)
            restartBtn.gameObject.SetActive (false);
#endif
            */
            restartBtn.gameObject.SetActive (false);

            // Кнопка "МЕНЮ"
            menuBtn.onClick.AddListener (() => OnButtonTouch (GAME_BUTTON.MENU, true));
            menuBtn.gameObject.SetActive (false);
            /*
            // Сброс мяча
            ballResetBtn.onClick.AddListener(() => OnButtonTouch(GAME_BUTTON._end, true));
            ballResetBtn.gameObject.SetActive(false);
            */
            //  Кнопки "УПРАВЛЕНИЕ" - главные
            var isDao = ConfDB.stat.use_dao_control == 1;
            singleButton.gameObject.SetActive (true);

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

        public void OnPointerDown (PointerEventData pdata)
        {
            if (point == null)
            {
                point = pdata;
                lastX = point.position.x;
                OnButtonTouch (GAME_BUTTON.MAIN, true);
                OnDragX (GAME_BUTTON.MAIN,lastX, point.position.x);
            }
        }

        public void OnPointerUp (PointerEventData pdata)
        {
            if (point != null)
            {
                leftBtPressed = false;
                rightBtPressed = false;
                point = null;

                OnButtonTouch (GAME_BUTTON.MAIN, false);
            }
        }

        float ScreenVel (float delta)
        {
            return delta / (float)Screen.width;
        }

        public float sens;

        void SolvePoint ()
        {
            if (point != null)
            {
                if (point.position.x - lastX > 5) // right
                {
                    OnButtonTouch (GAME_BUTTON.LEFT, false);
                    OnButtonTouch (GAME_BUTTON.RIGHT, true);
                }

                if (point.position.x - lastX < -5)
                {
                    OnButtonTouch (GAME_BUTTON.LEFT, false);
                    OnButtonTouch (GAME_BUTTON.RIGHT, true);
                }

                OnButtonTouchVel (GAME_BUTTON.MAIN, true, ScreenVel (point.position.x - lastX));

                OnButtonTouchVel (GAME_BUTTON.LEFT, false, 0);
                OnButtonTouchVel (GAME_BUTTON.RIGHT, false, 0);

                OnButtonTouch (GAME_BUTTON.MAIN, true);
                //lastX = Mathf.Lerp (lastX, point.position.x, sens);

                OnDragX (GAME_BUTTON.MAIN, lastX, point.position.x);
            }
            else
            {
                OnButtonTouch (GAME_BUTTON.LEFT, false);
                OnButtonTouch (GAME_BUTTON.RIGHT, false);
                OnButtonTouchVel (GAME_BUTTON.LEFT, false, 0);
                OnButtonTouchVel (GAME_BUTTON.RIGHT, false, 0);

                OnButtonTouch (GAME_BUTTON.MAIN, false);
                OnButtonTouchVel (GAME_BUTTON.MAIN, false, 0);
            }
        }

        void OnApplicationPause (bool pauseStatus)
        {
            G.SetPause (pauseStatus);
        }

        void Update ()
        {
            SolvePoint ();

#if (UNITY_EDITOR)
            OnButtonTouch (GAME_BUTTON.LEFT, Input.GetKey (KeyCode.A));
            OnButtonTouch (GAME_BUTTON.RIGHT, Input.GetKey (KeyCode.D));
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
                if (Input.GetKeyDown (KeyCode.Joystick1Button0) ||
                    Input.GetKeyDown (KeyCode.Joystick2Button0) ||
                    Input.GetKeyDown (KeyCode.Joystick3Button0)
                    )
                {
                    OnButtonTouch (GAME_BUTTON.MAIN, true);
                }
                if (Input.GetKeyUp (KeyCode.Joystick1Button0) ||
                    Input.GetKeyUp (KeyCode.Joystick2Button0) ||
                    Input.GetKeyUp (KeyCode.Joystick3Button0)
                    )
                {
                    OnButtonTouch (GAME_BUTTON.MAIN, false);
                }
            }

            if (Input.GetKeyDown (KeyCode.R))
            {
                OnButtonTouch (GAME_BUTTON.RESTART, true);
            }

            // Пауза по ВНЕШНЕЙ кнопке
            if (Input.GetKeyDown (KeyCode.Escape))
            {
                G.SetPause (!G.isPause);
            }
        }
    }
}