using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Выбор уровня
/// Все доступные уровни
/// </summary>
namespace RBGame
{
    public interface IStageSelector
    {
        IStagePlate AddStage();
        void OnBackButton(System.Action onStartClick);
        void ClearAll();
    }

    /// <summary>
    /// View
    /// </summary>
    public class Win_SelectStage : MonoBehaviour, IStageSelector, IWindow
    {
        public Transform gridPlateContainer;
        public Button backButton;

        [Header("prefab_link")]
        [SerializeField] GameObject stagePlaitMB;


        private void Awake()
        {
            new PR_StageSelector(this);
        }

        public IStagePlate AddStage()
        {
            var stage = Instantiate(stagePlaitMB).GetComponent<Wdg_StagePlate>();
            stage.transform.SetParent(gridPlateContainer, false);
            return stage;
        }

        public void ClearAll()
        {
            foreach (Transform child in gridPlateContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void OnBackButton(System.Action onStartClick)
        {
            backButton.onClick.AddListener(() => onStartClick());
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }



    /// <summary>
    /// Presenter
    /// </summary>
    public class PR_StageSelector : IWinPresenter
    {
        IStageSelector mViewContainer;
        System.Action dropAllSelection;

        public PR_StageSelector(IStageSelector viewContainer)
        {
            mViewContainer = viewContainer;

            //  "Назад", открыть главное меню
            mViewContainer.OnBackButton(() =>
            {
                UIManager.OpenWin(WINDOW.PROPS_MENU, null);
            });


            // Контейнер с плашками уровней
            mViewContainer.ClearAll();
            IStagePlate firstPlate = null;


            //  0 - первый бандл, пока только один


            foreach (var e in ConfDB.mapBundles[0].maps)
            {
                var stageMapID = e.map_id;

                // пред. результат
                var prevResult = StageManager.FindStageStat(stageMapID);

                var stagePlate = mViewContainer.AddStage();

                // расставим пред. результат
                stagePlate.SetCoinScore(prevResult.max_coin);
                stagePlate.SetStarScore(prevResult.max_star);
                stagePlate.SetRunCount(prevResult.run_count);
                stagePlate.SetNumber(e.num);
                stagePlate.SetEnable(prevResult.is_complete > 0);

                long retStageID = prevResult.id; // CLOSURE
                int retMapNUM = e.num; // CLOSURE
                stagePlate.SetListener(() =>
                {
                    dropAllSelection?.Invoke(); // change view
                    stagePlate.SelectIt(); // change view

                    StageManager.stageNUM = retMapNUM;
                    StageManager.stageID = retStageID;
                    G.stageFileName = StageManager.currStageFileName;
                    SceneLoader.Game();

                    //G.isTutorEnable = StageManager.stageNUM == 1; // если первый уровень - то тутор
                });

                /*
                // сразу подписываю кнопки на "сбросить выделение"
                dropAllSelection += stagePlate.UnselectIt;

                // для автовыбора первой плашки
                // вообще в идеале помнить на какой карте играли последний раз
                if (firstPlate == null)
                {
                    G.stageMapID = prevResult.id;
                    firstPlate = stagePlate;
                    firstPlate.SelectIt();
                }

                long retStageID = prevResult.id; // closure
                stagePlate.SetListener(() =>
                {

                    dropAllSelection(); // change view
                    stagePlate.SelectIt(); // change view

                    G.stageMapID = retStageID;
                });
                */
            }
        }

        public void Init(JSONNode data)
        {
        }
    }
}