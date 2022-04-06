using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RBGame
{

    /// <summary>
    /// Окно после удара. Рестарт/Продолжить/Реклама
    /// </summary>
    public class Win_PopImpact : MonoBehaviour, IWindow
    {
        [SerializeField] Button btnOutArea;
        [SerializeField] Button btnContinue;
        [SerializeField] Button btnRepeat;
        [SerializeField] Button btnMainMenu;
        [SerializeField] Text contCounter;
        [SerializeField] Transform imgRewardVideoIco;

        void Start()
        {
            btnOutArea.onClick.AddListener(() =>
            {
                /*
                if (G.freezeGameByImpact)
                    return;
                */
                G.SetPause(false);
            });

            // начать сначала 
            btnRepeat.onClick.AddListener(() => {
                /*
                if (Random.value < 1)
                { 
                
                }
                else*/
                SceneLoader.Game();
            });

            // в главное
            btnMainMenu.onClick.AddListener(() => {
                SceneLoader.Menu();
            });

            //  Кнопка "ПРОДОЛЖИТЬ"
            /*
            if (AdsRevardVideo.inst)
                AdsRevardVideo.inst.OnEarnedReward += Inst_OnEarnedReward;
            
            //  Если "врезались" И "не редактор"
            if (!G.isEditMode)
            {
                if (AdsRevardVideo.inst && AdsRevardVideo.inst.IsLoadRewardVideo())
                {
                    StartCoroutine(TimeCounter());
                }
                else
                {
                    //  Если видео недоступно, то продолжить нельзя
                    btnContinue.gameObject.SetActive(false);
                }
            }
            else
            {
                //  мы просто на "паузе", кнопку РЕВАРД не показывать
                imgRewardVideoIco.gameObject.SetActive(false);
                btnContinue.onClick.AddListener(() => Continue());
                contCounter.text = "CONTINUE";
            }*/
        }

        void Continue()
        {
            /*
            gameObject.SetActive(false);
            if (G.freezeGameByImpact)
                G.ResetImpact();
            G.SetPause(false);
            */
        }

        private void Inst_OnEarnedReward()
        {
            btnContinue.interactable = true;
            imgRewardVideoIco.gameObject.SetActive(false);
            btnContinue.onClick.RemoveAllListeners();
            btnContinue.onClick.AddListener(() => Continue());
        }

        IEnumerator TimeCounter()
        {
            //  Сколько секунд блочить кнопку
            int WTimer = (int)ConfDB.game.ContinueDelaySec;
            
            btnContinue.interactable = false;

            for (int i = 0; i < WTimer; i++)
            {
                contCounter.text = "WAIT... " + (WTimer - i);
                yield return new WaitForSeconds(1);
            }

            btnContinue.interactable = true; 
            contCounter.text = "CONTINUE";
#if (ZX_RELEASE)
            // После отсчета по клику будет "СМОТРЕТЬ РЕКЛАМУ"
            btnContinue.onClick.AddListener(() => 
            {
                /*
                AdsRevardVideo.inst.PlayRewardVideo();
                btnContinue.interactable = false; // если недосмотрит кнопка будет заблокирована
                */
            });
#else
            //  В режиме разработки просто даём продолжить
            btnContinue.interactable = true;
#endif
        }

        
        public void DestroyWindow()
        {
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            /*
            if (AdsRevardVideo.inst)
                AdsRevardVideo.inst.OnEarnedReward -= Inst_OnEarnedReward;
                */
        }
    }
}