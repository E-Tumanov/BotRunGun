using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIAnimatorCore;
using UnityEngine.Audio;

namespace RBGame
{
    public class Win_PropsMenu : MonoBehaviour, IWindow
    {
        [SerializeField] AudioMixer master;

        [SerializeField] Button btnControlSwitch;
        [SerializeField] Button btnCloseProps;
        [SerializeField] Button btnSoundSwitch;
        [SerializeField] Button btnVibroSwitch;
        [SerializeField] Button btnStageSelectorWin;

        [SerializeField] Transform oneTapControl;
        [SerializeField] Transform sideTapControl;

        [SerializeField] Transform lockSound;
        [SerializeField] Transform lockVibro;

        void Start()
        {
            btnCloseProps.onClick.AddListener(() =>
            {
                UIManager.OpenWin(WINDOW.MAIN_MENU, null);

            });

            btnStageSelectorWin.onClick.AddListener(() =>
            {
                UIManager.OpenWin(WINDOW.STAGE_SELECT_GRID, null);
            });

            // управление "ДАО"
            btnControlSwitch.onClick.AddListener(() =>
            {
                if (ConfDB.stat.use_dao_control != 0)
                    ConfDB.stat.use_dao_control = 0;
                else
                    ConfDB.stat.use_dao_control = 1;

                SetDao();
                ConfDB.SaveStat();
            });
            

            btnSoundSwitch.onClick.AddListener(() =>
            {
                if (ConfDB.stat.use_sound > 0)
                    ConfDB.stat.use_sound = 0;
                else
                    ConfDB.stat.use_sound = 1;

                SetSound();
                ConfDB.SaveStat();
            });

            btnVibroSwitch.onClick.AddListener(() =>
            {
                if (ConfDB.stat.use_vibro > 0)
                    ConfDB.stat.use_vibro = 0;
                else
                    ConfDB.stat.use_vibro = 1;
                SetVibro();
                ConfDB.SaveStat();
            });

            SetDao();
            SetSound();
            SetVibro();
        }

        void SetSound()
        {
            lockSound.gameObject.SetActive(ConfDB.stat.use_sound == 0);
            // вот тут неплохо про миксер
            //https://gamedevbeginner.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
            if (ConfDB.stat.use_sound == 1)
                AudioListener.volume = 1;
            else
                AudioListener.volume = 0;
        }

        void SetVibro()
        {
            lockVibro.gameObject.SetActive(ConfDB.stat.use_vibro == 0);
        }

        void SetDao()
        {
            oneTapControl.gameObject.SetActive(ConfDB.stat.use_dao_control != 0);
            sideTapControl.gameObject.SetActive(ConfDB.stat.use_dao_control == 0);
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }
}